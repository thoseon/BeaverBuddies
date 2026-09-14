using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
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

namespace Timberborn.SoilContaminationSystem;

[MapEditorTickable]
internal class SoilContaminationSimulator : ISaveableSingleton, ILoadableSingleton, ITickableSingleton, IParallelTickableSingleton
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
		MaxTerrainThreadSafeColumnCountChanged,
		ColumnMovedUp,
		ColumnMovedDown,
		ColumnReset
	}

	private static readonly SingletonKey SoilContaminationSimulatorKey = new SingletonKey("SoilContaminationSimulator");

	private static readonly PropertyKey<int> SizeKey = new PropertyKey<int>("Size");

	private static readonly PropertyKey<PackedList<float>> ContaminationCandidatesKey = new PropertyKey<PackedList<float>>("ContaminationCandidates");

	private static readonly PropertyKey<PackedList<float>> ContaminationLevelsKey = new PropertyKey<PackedList<float>>("ContaminationLevels");

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapIndexService _mapIndexService;

	private readonly ISpecService _specService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly SoilBarrierMap _soilBarrierMap;

	private readonly FloatPackedListSerializer _floatPackedListSerializer;

	private readonly IThreadSafeColumnTerrainMap _threadSafeColumnTerrainMap;

	private readonly SoilContaminationSimulationTaskStarter _soilContaminationSimulationTaskStarter;

	private readonly SimulationController _simulationController;

	private readonly ITerrainService _terrainService;

	private readonly ITickableSingletonService _tickableSingletonService;

	private readonly TickOnlyArrayService _tickOnlyArrayService;

	private TickOnlyArray<float> _contaminationCandidates;

	private TickOnlyArray<float> _lastTickContaminationCandidates;

	private TickOnlyArray<float> _contaminationLevels;

	private TickOnlyArray<bool> _contaminationsChangedLastTick;

	private int _verticalStride;

	private float _verticalCostModifier;

	private readonly List<Action> _actions = new List<Action>();

	private readonly List<TerrainHeightChange> _terrainHeightChanges = new List<TerrainHeightChange>();

	public ReadOnlySpan<float> ContaminationLevels => _contaminationLevels.GetReadOnlySpan();

	public ReadOnlySpan<bool> ContaminationsChangedLastTick => _contaminationsChangedLastTick.GetReadOnlySpan();

	public SoilContaminationSimulator(ISingletonLoader singletonLoader, MapIndexService mapIndexService, ISpecService specService, FloatPackedListSerializer floatPackedListSerializer, IThreadSafeWaterMap threadSafeWaterMap, SoilBarrierMap soilBarrierMap, IThreadSafeColumnTerrainMap threadSafeColumnTerrainMap, SoilContaminationSimulationTaskStarter soilContaminationSimulationTaskStarter, SimulationController simulationController, ITerrainService terrainService, ITickableSingletonService tickableSingletonService, TickOnlyArrayService tickOnlyArrayService)
	{
		_singletonLoader = singletonLoader;
		_mapIndexService = mapIndexService;
		_specService = specService;
		_floatPackedListSerializer = floatPackedListSerializer;
		_threadSafeWaterMap = threadSafeWaterMap;
		_soilBarrierMap = soilBarrierMap;
		_threadSafeColumnTerrainMap = threadSafeColumnTerrainMap;
		_soilContaminationSimulationTaskStarter = soilContaminationSimulationTaskStarter;
		_simulationController = simulationController;
		_terrainService = terrainService;
		_tickableSingletonService = tickableSingletonService;
		_tickOnlyArrayService = tickOnlyArrayService;
	}

	[BackwardCompatible(2023, 9, 22, Compatibility.Map)]
	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
		int maxColumnCount = _threadSafeColumnTerrainMap.MaxColumnCount;
		int size = _verticalStride * maxColumnCount;
		_lastTickContaminationCandidates = _tickOnlyArrayService.Create<float>(size);
		_contaminationsChangedLastTick = _tickOnlyArrayService.Create<bool>(size);
		_contaminationCandidates = _tickOnlyArrayService.Create<float>(size);
		_contaminationLevels = _tickOnlyArrayService.Create<float>(size);
		if (_singletonLoader.TryGetSingleton(SoilContaminationSimulatorKey, out var objectLoader))
		{
			int levels = ((!objectLoader.Has(SizeKey)) ? 1 : Math.Min(objectLoader.Get(SizeKey), maxColumnCount));
			PackedList<float> packedList = objectLoader.Get(ContaminationCandidatesKey, _floatPackedListSerializer);
			_mapIndexService.Unpack(packedList, _contaminationCandidates.GetSpan(), levels);
			PackedList<float> packedList2 = objectLoader.Get(ContaminationLevelsKey, _floatPackedListSerializer);
			Span<float> span = _contaminationLevels.GetSpan();
			_mapIndexService.Unpack(packedList2, span, levels);
		}
		else
		{
			BackwardCompatibleLoad();
		}
		SoilContaminationSimulatorSpec singleSpec = _specService.GetSingleSpec<SoilContaminationSimulatorSpec>();
		_verticalCostModifier = singleSpec.VerticalSpreadCostMultiplier / (float)singleSpec.MaxRangeFromSource;
		_verticalStride = _mapIndexService.VerticalStride;
		_terrainService.TerrainHeightChanged += delegate(object _, TerrainHeightChangeEventArgs eventArgs)
		{
			_terrainHeightChanges.Add(eventArgs.Change);
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
		_tickableSingletonService.ForcedParallelTickFinished += delegate
		{
			Tick();
		};
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(SoilContaminationSimulatorKey);
		int maxColumnCount = _threadSafeColumnTerrainMap.MaxColumnCount;
		singleton.Set(SizeKey, maxColumnCount);
		ReadOnlySpan<float> readOnlySpan = _contaminationCandidates.GetReadOnlySpan();
		singleton.Set(ContaminationCandidatesKey, _mapIndexService.Pack(readOnlySpan, maxColumnCount), _floatPackedListSerializer);
		ReadOnlySpan<float> readOnlySpan2 = _contaminationLevels.GetReadOnlySpan();
		singleton.Set(ContaminationLevelsKey, _mapIndexService.Pack(readOnlySpan2, maxColumnCount), _floatPackedListSerializer);
	}

	public void Tick()
	{
		ProcessActions();
		ProcessTerrainHeightChanges();
		Reset();
	}

	public void StartParallelTick()
	{
		_soilContaminationSimulationTaskStarter.StartTask(_contaminationLevels.GetArray(), _contaminationsChangedLastTick.GetArray(), _contaminationCandidates.GetArray(), _lastTickContaminationCandidates.GetArray(), _threadSafeWaterMap.ColumnCounts, _threadSafeWaterMap.WaterColumns, _threadSafeColumnTerrainMap.ColumnCounts, _threadSafeColumnTerrainMap.TerrainColumns, _soilBarrierMap.ContaminationBarriers, _soilBarrierMap.AboveMoistureBarriers);
	}

	private void BackwardCompatibleLoad()
	{
		int maxIndex = _mapIndexService.MaxIndex;
		SingletonKey key = new SingletonKey("SoilPollutionSimulator");
		PropertyKey<PackedList<float>> key2 = new PropertyKey<PackedList<float>>("PollutionCandidates");
		PropertyKey<PackedList<float>> key3 = new PropertyKey<PackedList<float>>("PollutionLevels");
		if (_singletonLoader.TryGetSingleton(key, out var objectLoader))
		{
			_contaminationCandidates = _tickOnlyArrayService.Create<float>(maxIndex);
			if (objectLoader.Has(key2))
			{
				PackedList<float> packedList = objectLoader.Get(key2, _floatPackedListSerializer);
				_mapIndexService.Unpack(packedList, _contaminationCandidates.GetSpan());
			}
			PackedList<float> packedList2 = objectLoader.Get(key3, _floatPackedListSerializer);
			_mapIndexService.Unpack(packedList2, _contaminationLevels.GetSpan());
		}
	}

	private void AddAction(ActionType actionType, int value)
	{
		_actions.Add(new Action(actionType, value));
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
			UpdateContaminationFromHeightChange(_terrainHeightChanges[i]);
		}
		_terrainHeightChanges.Clear();
	}

	private void Reset()
	{
		if (_simulationController.ShouldResetSimulation)
		{
			_contaminationCandidates.GetSpan().Clear();
			_contaminationLevels.GetSpan().Clear();
			_contaminationsChangedLastTick.GetSpan().Clear();
		}
	}

	private void ProcessAction(Action action)
	{
		switch (action.Type)
		{
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

	private void UpdateContaminationFromHeightChange(TerrainHeightChange terrainHeightChange)
	{
		Span<bool> span = _contaminationsChangedLastTick.GetSpan();
		Span<float> span2 = _contaminationLevels.GetSpan();
		Span<float> span3 = _contaminationCandidates.GetSpan();
		bool setTerrain = terrainHeightChange.SetTerrain;
		int num = (setTerrain ? (terrainHeightChange.To + 1) : terrainHeightChange.From);
		int index2D = _mapIndexService.CellToIndex(terrainHeightChange.Coordinates);
		if (_threadSafeColumnTerrainMap.TryGetIndexAtOrAboveCeiling(index2D, num, out var index3D))
		{
			span[index3D] = true;
			if (setTerrain)
			{
				int num2 = num - terrainHeightChange.From;
				float val = span2[index3D] - _verticalCostModifier * (float)num2;
				span2[index3D] = (span3[index3D] = Math.Max(0f, val));
			}
		}
	}

	private void ResizeTerrainBasedArrays(int maxColumnCount)
	{
		int newSize = maxColumnCount * _verticalStride;
		_lastTickContaminationCandidates.Resize(newSize);
		_contaminationsChangedLastTick.Resize(newSize);
		_contaminationCandidates.Resize(newSize);
		_contaminationLevels.Resize(newSize);
	}

	private void MoveColumn(int target, int source)
	{
		Span<float> span = _contaminationLevels.GetSpan();
		span[target] = span[source];
		Span<float> span2 = _contaminationCandidates.GetSpan();
		span2[target] = span2[source];
		Span<float> span3 = _lastTickContaminationCandidates.GetSpan();
		span3[target] = span3[source];
		_contaminationsChangedLastTick.GetSpan()[target] = true;
	}

	private void ResetColumn(int index3D)
	{
		_contaminationLevels.GetSpan()[index3D] = 0f;
		_contaminationCandidates.GetSpan()[index3D] = 0f;
		_lastTickContaminationCandidates.GetSpan()[index3D] = 0f;
		_contaminationsChangedLastTick.GetSpan()[index3D] = true;
	}
}
