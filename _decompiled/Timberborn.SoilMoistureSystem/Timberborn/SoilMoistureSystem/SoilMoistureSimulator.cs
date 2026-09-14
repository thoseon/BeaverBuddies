using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;
using Timberborn.SimulationSystem;
using Timberborn.SingletonSystem;
using Timberborn.SoilBarrierSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.SoilMoistureSystem;

[MapEditorTickable]
internal class SoilMoistureSimulator : ISaveableSingleton, ILoadableSingleton, ITickableSingleton, IParallelTickableSingleton
{
	private readonly struct Action
	{
		public ActionType Type { get; }

		public int Value { get; }

		public Action(ActionType type, int value)
		{
			Type = type;
			Value = value;
		}
	}

	private enum ActionType
	{
		MaxWaterColumnCountChange,
		MaxTerrainThreadSafeColumnCountChanged,
		ColumnMovedUp,
		ColumnMovedDown,
		ColumnReset
	}

	private static readonly SingletonKey SoilMoistureSimulatorKey = new SingletonKey("SoilMoistureSimulator");

	private static readonly PropertyKey<int> SizeKey = new PropertyKey<int>("Size");

	private static readonly PropertyKey<PackedList<float>> MoistureLevelsKey = new PropertyKey<PackedList<float>>("MoistureLevels");

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapIndexService _mapIndexService;

	private readonly ISpecService _specService;

	private readonly FloatPackedListSerializer _floatPackedListSerializer;

	private readonly SoilMoistureSimulationTaskStarter _soilMoistureSimulationTaskStarter;

	private readonly SoilBarrierMap _soilBarrierMap;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly IThreadSafeColumnTerrainMap _threadSafeColumnTerrainMap;

	private readonly SimulationController _simulationController;

	private readonly ITerrainService _terrainService;

	private readonly ITickableSingletonService _tickableSingletonService;

	private readonly WaterEvaporationMap _waterEvaporationMap;

	private readonly TickOnlyArrayService _tickOnlyArrayService;

	private TickOnlyArray<float> _moistureLevels;

	private TickOnlyArray<bool> _moistureLevelsChangedLastTick;

	private TickOnlyArray<float> _lastTickMoistureLevels;

	private TickOnlyArray<byte> _wateredNeighbours;

	private TickOnlyArray<byte> _clusterSaturations;

	private int _verticalStride;

	private int _verticalSpreadCostMultiplier;

	private readonly List<Action> _actions = new List<Action>();

	private readonly List<TerrainHeightChange> _terrainHeightChanges = new List<TerrainHeightChange>();

	public ReadOnlySpan<float> MoistureLevels => _moistureLevels.GetReadOnlySpan();

	public ReadOnlySpan<bool> MoistureLevelsChangedLastTick => _moistureLevelsChangedLastTick.GetReadOnlySpan();

	public SoilMoistureSimulator(ISingletonLoader singletonLoader, MapIndexService mapIndexService, ISpecService specService, FloatPackedListSerializer floatPackedListSerializer, SoilMoistureSimulationTaskStarter soilMoistureSimulationTaskStarter, SoilBarrierMap soilBarrierMap, IThreadSafeWaterMap threadSafeWaterMap, IThreadSafeColumnTerrainMap threadSafeColumnTerrainMap, SimulationController simulationController, ITerrainService terrainService, ITickableSingletonService tickableSingletonService, WaterEvaporationMap waterEvaporationMap, TickOnlyArrayService tickOnlyArrayService)
	{
		_singletonLoader = singletonLoader;
		_mapIndexService = mapIndexService;
		_specService = specService;
		_floatPackedListSerializer = floatPackedListSerializer;
		_soilMoistureSimulationTaskStarter = soilMoistureSimulationTaskStarter;
		_soilBarrierMap = soilBarrierMap;
		_threadSafeWaterMap = threadSafeWaterMap;
		_threadSafeColumnTerrainMap = threadSafeColumnTerrainMap;
		_simulationController = simulationController;
		_terrainService = terrainService;
		_tickableSingletonService = tickableSingletonService;
		_waterEvaporationMap = waterEvaporationMap;
		_tickOnlyArrayService = tickOnlyArrayService;
	}

	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
		int maxColumnCount = _threadSafeColumnTerrainMap.MaxColumnCount;
		int size = _verticalStride * maxColumnCount;
		_moistureLevels = _tickOnlyArrayService.Create<float>(size);
		_lastTickMoistureLevels = _tickOnlyArrayService.Create<float>(size);
		_moistureLevelsChangedLastTick = _tickOnlyArrayService.Create<bool>(size);
		_wateredNeighbours = _tickOnlyArrayService.Create<byte>(_verticalStride);
		_clusterSaturations = _tickOnlyArrayService.Create<byte>(_verticalStride);
		if (_singletonLoader.TryGetSingleton(SoilMoistureSimulatorKey, out var objectLoader))
		{
			int val = ((!objectLoader.Has(SizeKey)) ? 1 : objectLoader.Get(SizeKey));
			PackedList<float> packedList = objectLoader.Get(MoistureLevelsKey, _floatPackedListSerializer);
			Span<float> span = _moistureLevels.GetSpan();
			_mapIndexService.Unpack(packedList, span, Math.Min(val, maxColumnCount));
		}
		SoilMoistureSimulatorSpec singleSpec = _specService.GetSingleSpec<SoilMoistureSimulatorSpec>();
		_verticalSpreadCostMultiplier = singleSpec.VerticalSpreadCostMultiplier;
		_threadSafeWaterMap.MaxWaterColumnCountChanged += delegate(object _, int i)
		{
			AddAction(ActionType.MaxWaterColumnCountChange, i);
		};
		_threadSafeColumnTerrainMap.MaxTerrainColumnCountChanged += delegate(object _, int i)
		{
			AddAction(ActionType.MaxTerrainThreadSafeColumnCountChanged, i);
		};
		_threadSafeColumnTerrainMap.ColumnMovedUp += delegate(object _, int i)
		{
			AddAction(ActionType.ColumnMovedUp, i);
		};
		_threadSafeColumnTerrainMap.ColumnMovedDown += delegate(object _, int i)
		{
			AddAction(ActionType.ColumnMovedDown, i);
		};
		_threadSafeColumnTerrainMap.ColumnReset += delegate(object _, int i)
		{
			AddAction(ActionType.ColumnReset, i);
		};
		_terrainService.TerrainHeightChanged += delegate(object _, TerrainHeightChangeEventArgs eventArgs)
		{
			_terrainHeightChanges.Add(eventArgs.Change);
		};
		_tickableSingletonService.ForcedParallelTickFinished += delegate
		{
			Tick();
		};
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(SoilMoistureSimulatorKey);
		int maxColumnCount = _threadSafeColumnTerrainMap.MaxColumnCount;
		singleton.Set(SizeKey, maxColumnCount);
		singleton.Set(MoistureLevelsKey, _mapIndexService.Pack(_moistureLevels.GetReadOnlySpan(), maxColumnCount), _floatPackedListSerializer);
	}

	public void Tick()
	{
		ProcessActions();
		ProcessTerrainHeightChanges();
		Reset();
	}

	public void StartParallelTick()
	{
		_soilMoistureSimulationTaskStarter.StartTask(_wateredNeighbours.GetArray(), _clusterSaturations.GetArray(), _moistureLevels.GetArray(), _moistureLevelsChangedLastTick.GetArray(), _lastTickMoistureLevels.GetArray(), _waterEvaporationMap.UnsafeEvaporationModifiers, _soilBarrierMap.FullMoistureBarriers, _soilBarrierMap.AboveMoistureBarriers, _threadSafeWaterMap.ColumnCounts, _threadSafeWaterMap.WaterColumns, _threadSafeColumnTerrainMap.ColumnCounts, _threadSafeColumnTerrainMap.TerrainColumns);
	}

	private void AddAction(ActionType actionType, int i)
	{
		_actions.Add(new Action(actionType, i));
	}

	private void ProcessActions()
	{
		foreach (Action action in _actions)
		{
			ProcessAction(action);
		}
		_actions.Clear();
	}

	private void ProcessTerrainHeightChanges()
	{
		for (int i = 0; i < _terrainHeightChanges.Count; i++)
		{
			UpdateMoistureFromHeightChange(_terrainHeightChanges[i]);
		}
		_terrainHeightChanges.Clear();
	}

	private void Reset()
	{
		if (_simulationController.ShouldResetSimulation)
		{
			_wateredNeighbours.GetSpan().Clear();
			_clusterSaturations.GetSpan().Clear();
			_moistureLevels.GetSpan().Clear();
			_moistureLevelsChangedLastTick.GetSpan().Clear();
		}
	}

	private void ProcessAction(Action action)
	{
		switch (action.Type)
		{
		case ActionType.MaxWaterColumnCountChange:
			ResizeWaterBasedArrays(action.Value);
			break;
		case ActionType.MaxTerrainThreadSafeColumnCountChanged:
			ResizeTerrainBasedArrays(action.Value);
			break;
		case ActionType.ColumnMovedUp:
			MoveColumn(action.Value, action.Value - _verticalStride);
			break;
		case ActionType.ColumnMovedDown:
			MoveColumn(action.Value, action.Value + _verticalStride);
			break;
		case ActionType.ColumnReset:
			ResetColumn(action.Value);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void UpdateMoistureFromHeightChange(TerrainHeightChange terrainHeightChange)
	{
		Span<float> span = _moistureLevels.GetSpan();
		Span<bool> span2 = _moistureLevelsChangedLastTick.GetSpan();
		bool setTerrain = terrainHeightChange.SetTerrain;
		int num = (setTerrain ? (terrainHeightChange.To + 1) : terrainHeightChange.From);
		int index2D = _mapIndexService.CellToIndex(terrainHeightChange.Coordinates);
		if (_threadSafeColumnTerrainMap.TryGetIndexAtOrAboveCeiling(index2D, num, out var index3D))
		{
			span2[index3D] = true;
			if (setTerrain)
			{
				int num2 = num - terrainHeightChange.From;
				float val = span[index3D] - (float)(_verticalSpreadCostMultiplier * num2);
				span[index3D] = Math.Max(0f, val);
			}
		}
	}

	private void ResizeWaterBasedArrays(int maxColumnCount)
	{
		int newSize = maxColumnCount * _verticalStride;
		_wateredNeighbours.Resize(newSize);
		_clusterSaturations.Resize(newSize);
	}

	private void ResizeTerrainBasedArrays(int maxColumnCount)
	{
		int newSize = maxColumnCount * _verticalStride;
		_moistureLevels.Resize(newSize);
		_lastTickMoistureLevels.Resize(newSize);
		_moistureLevelsChangedLastTick.Resize(newSize);
	}

	private void MoveColumn(int target, int source)
	{
		Span<float> span = _moistureLevels.GetSpan();
		span[target] = span[source];
		Span<float> span2 = _lastTickMoistureLevels.GetSpan();
		span2[target] = span2[source];
		_moistureLevelsChangedLastTick.GetSpan()[target] = true;
	}

	private void ResetColumn(int index3D)
	{
		_moistureLevels.GetSpan()[index3D] = 0f;
		_lastTickMoistureLevels.GetSpan()[index3D] = 0f;
		_moistureLevelsChangedLastTick.GetSpan()[index3D] = true;
	}
}
