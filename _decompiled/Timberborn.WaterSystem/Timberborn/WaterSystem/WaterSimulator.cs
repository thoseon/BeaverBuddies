using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;
using Timberborn.SimulationSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterSystem;

[MapEditorTickable]
internal class WaterSimulator : ISaveableSingleton, ILoadableSingleton, IPostLoadableSingleton, ITickableSingleton, IParallelTickableSingleton
{
	private readonly struct Modification
	{
		public bool Added { get; }

		public Vector3Int Coordinates { get; }

		public ChangeType ChangeType { get; }

		public Modification(bool added, Vector3Int coordinates, ChangeType changeType)
		{
			Added = added;
			Coordinates = coordinates;
			ChangeType = changeType;
		}
	}

	private enum ChangeType
	{
		FullObstacle,
		HorizontalObstacle,
		FullCellBlock
	}

	private static readonly SingletonKey WaterMapKey = new SingletonKey("WaterMapNew");

	private static readonly PropertyKey<int> LevelsKey = new PropertyKey<int>("Levels");

	private static readonly PropertyKey<PackedList<WaterColumn>> WaterColumnsKey = new PropertyKey<PackedList<WaterColumn>>("WaterColumns");

	private static readonly PropertyKey<PackedList<ColumnOutflows>> ColumnOutflowsKey = new PropertyKey<PackedList<ColumnOutflows>>("ColumnOutflows");

	private readonly MapIndexService _mapIndexService;

	private readonly ITerrainService _terrainService;

	private readonly ISingletonLoader _singletonLoader;

	private readonly ColumnOutflowsPackedListSerializer _columnOutflowsPackedListSerializer;

	private readonly WaterColumnPackedListSerializer _waterColumnPackedListSerializer;

	private readonly MutableWaterColumnRetriever _mutableWaterColumnRetriever;

	private readonly SimulationController _simulationController;

	private readonly WaterMapLoader _waterMapLoader;

	private readonly ITickableSingletonService _tickableSingletonService;

	private readonly WaterSourceRegistry _waterSourceRegistry;

	private readonly IThreadSafeWaterEvaporationMap _threadSafeWaterEvaporationMap;

	private readonly FlowLimiterService _flowLimiterService;

	private readonly WaterSimulationTaskStarter _waterSimulationTaskStarter;

	private readonly WaterChangeService _waterChangeService;

	private readonly TickOnlyArrayService _tickOnlyArrayService;

	private readonly WaterSimulationMigrator _waterSimulationMigrator;

	private TickOnlyArray<byte> _columnCounts;

	private TickOnlyArray<WaterColumn> _waterColumns;

	private TickOnlyArray<ColumnOutflows> _outflows;

	private TickOnlyArray<byte> _horizontalObstacles;

	private TickOnlyArray<WaterFlow> _baseLevelFlows;

	private TickOnlyArray<List<DirectedFlow>> _directedFlows;

	private TickOnlyArray<Diffusions> _baseLevelDiffusions;

	private TickOnlyArray<List<TargetedDiffusion>> _targetedDiffusions;

	private TickOnlyArray<float> _contaminationsBuffer;

	private TickOnlyArray<byte> _targetedDiffusionCount;

	private Vector3Int _mapSize;

	private int _stride;

	private int _verticalStride;

	private int _maxColumnHeight;

	private readonly Queue<Modification> _modifications = new Queue<Modification>();

	public int MaxColumnCount { get; private set; } = 1;

	public bool AnyColumnChanged { get; private set; }

	public ReadOnlySpan<WaterColumn> WaterColumns => _waterColumns.GetReadOnlySpan();

	public ReadOnlySpan<ColumnOutflows> Outflows => _outflows.GetReadOnlySpan();

	public ReadOnlySpan<byte> ColumnCounts => _columnCounts.GetReadOnlySpan();

	public WaterSimulator(MapIndexService mapIndexService, ITerrainService terrainService, ISingletonLoader singletonLoader, ColumnOutflowsPackedListSerializer columnOutflowsPackedListSerializer, WaterColumnPackedListSerializer waterColumnPackedListSerializer, MutableWaterColumnRetriever mutableWaterColumnRetriever, SimulationController simulationController, ITickableSingletonService tickableSingletonService, WaterMapLoader waterMapLoader, WaterSourceRegistry waterSourceRegistry, IThreadSafeWaterEvaporationMap threadSafeWaterEvaporationMap, FlowLimiterService flowLimiterService, WaterSimulationTaskStarter waterSimulationTaskStarter, WaterChangeService waterChangeService, TickOnlyArrayService tickOnlyArrayService, WaterSimulationMigrator waterSimulationMigrator)
	{
		_mapIndexService = mapIndexService;
		_terrainService = terrainService;
		_singletonLoader = singletonLoader;
		_columnOutflowsPackedListSerializer = columnOutflowsPackedListSerializer;
		_waterColumnPackedListSerializer = waterColumnPackedListSerializer;
		_mutableWaterColumnRetriever = mutableWaterColumnRetriever;
		_simulationController = simulationController;
		_tickableSingletonService = tickableSingletonService;
		_waterMapLoader = waterMapLoader;
		_waterSourceRegistry = waterSourceRegistry;
		_threadSafeWaterEvaporationMap = threadSafeWaterEvaporationMap;
		_flowLimiterService = flowLimiterService;
		_waterSimulationTaskStarter = waterSimulationTaskStarter;
		_waterChangeService = waterChangeService;
		_tickOnlyArrayService = tickOnlyArrayService;
		_waterSimulationMigrator = waterSimulationMigrator;
	}

	public void Load()
	{
		_mapSize = _mapIndexService.TotalSize;
		_maxColumnHeight = _mapSize.z + 1;
		_stride = _mapIndexService.Stride;
		_verticalStride = _mapIndexService.VerticalStride;
		_outflows = _tickOnlyArrayService.Create<ColumnOutflows>(_verticalStride);
		_horizontalObstacles = _tickOnlyArrayService.Create<byte>(_verticalStride * _maxColumnHeight);
		int maxIndex = _mapIndexService.MaxIndex;
		_baseLevelFlows = _tickOnlyArrayService.Create<WaterFlow>(maxIndex);
		_baseLevelDiffusions = _tickOnlyArrayService.Create<Diffusions>(maxIndex);
		_directedFlows = _tickOnlyArrayService.Create<List<DirectedFlow>>(maxIndex);
		_targetedDiffusions = _tickOnlyArrayService.Create<List<TargetedDiffusion>>(maxIndex);
		Span<List<DirectedFlow>> span = _directedFlows.GetSpan();
		Span<List<TargetedDiffusion>> span2 = _targetedDiffusions.GetSpan();
		for (int i = 0; i < maxIndex; i++)
		{
			span[i] = new List<DirectedFlow>(0);
			span2[i] = new List<TargetedDiffusion>(0);
		}
		_contaminationsBuffer = _tickOnlyArrayService.Create<float>(_verticalStride);
		_targetedDiffusionCount = _tickOnlyArrayService.Create<byte>(_verticalStride);
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
		_tickableSingletonService.ForcedParallelTickFinished += delegate
		{
			ProcessModifications();
		};
		CreateColumns();
	}

	public void PostLoad()
	{
		Update();
		if (_singletonLoader.TryGetSingleton(WaterMapKey, out var objectLoader))
		{
			int levels = objectLoader.Get(LevelsKey);
			PackedList<WaterColumn> packedList = objectLoader.Get(WaterColumnsKey, _waterColumnPackedListSerializer);
			PackedList<ColumnOutflows> packedList2 = objectLoader.Get(ColumnOutflowsKey, _columnOutflowsPackedListSerializer);
			WaterColumn[] array = _mapIndexService.Unpack(packedList, levels);
			ColumnOutflows[] array2 = _mapIndexService.Unpack(packedList2, levels);
			int num = array.Length;
			ReadOnlySpan<byte> readOnlySpan = _columnCounts.GetReadOnlySpan();
			Span<WaterColumn> span = _waterColumns.GetSpan();
			Span<ColumnOutflows> span2 = _outflows.GetSpan();
			foreach (int item in _mapIndexService.Indices2D)
			{
				for (int i = 0; i < readOnlySpan[item]; i++)
				{
					int num2 = i * _verticalStride + item;
					ref WaterColumn reference = ref span[num2];
					if (num2 < num)
					{
						ref WaterColumn reference2 = ref array[num2];
						reference.WaterDepth = reference2.WaterDepth;
						reference.OldWaterDepth = reference2.OldWaterDepth;
						reference.Contamination = reference2.Contamination;
						reference.Overflow = reference2.Overflow;
						span2[num2] = array2[num2];
					}
				}
			}
		}
		else
		{
			_waterMapLoader.Load(_waterColumns.GetSpan(), _outflows.GetSpan());
		}
		_waterSimulationMigrator.MigrateOutflows(_outflows.GetSpan());
		AnyColumnChanged = true;
	}

	public void Tick()
	{
		Update();
	}

	public void StartParallelTick()
	{
		_waterSimulationTaskStarter.Simulate(_directedFlows.GetArray(), _baseLevelFlows.GetArray(), _waterColumns.GetArray(), _outflows.GetArray(), _contaminationsBuffer.GetArray(), _baseLevelDiffusions.GetArray(), _targetedDiffusionCount.GetArray(), _targetedDiffusions.GetArray(), _waterChangeService.RemovedWaterUnsafe, new ReadOnlyArray<byte>(_columnCounts.GetArray()), _flowLimiterService.LimitedDirections, _flowLimiterService.HeightLimits, _flowLimiterService.InflowLimits, _threadSafeWaterEvaporationMap.EvaporationModifiers, _waterSourceRegistry.ThreadSafeWaterSources, _waterChangeService.ThreadSafeWaterChanges);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		int num = 1;
		ReadOnlySpan<byte> readOnlySpan = _columnCounts.GetReadOnlySpan();
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = readOnlySpan[i];
			if (b > num)
			{
				num = b;
			}
		}
		IObjectSaver singleton = singletonSaver.GetSingleton(WaterMapKey);
		singleton.Set(LevelsKey, num);
		singleton.Set(WaterColumnsKey, _mapIndexService.Pack(_waterColumns.GetReadOnlySpan(), num), _waterColumnPackedListSerializer);
		singleton.Set(ColumnOutflowsKey, _mapIndexService.Pack(_outflows.GetReadOnlySpan(), num), _columnOutflowsPackedListSerializer);
	}

	public void AddFullObstacle(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: true, coordinates, ChangeType.FullObstacle));
	}

	public void RemoveFullObstacle(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: false, coordinates, ChangeType.FullObstacle));
	}

	public void AddHorizontalObstacle(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: true, coordinates, ChangeType.HorizontalObstacle));
	}

	public void RemoveHorizontalObstacle(Vector3Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: false, coordinates, ChangeType.HorizontalObstacle));
	}

	public void FullyBlockCell(Vector2Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: true, coordinates.XYZ(), ChangeType.FullCellBlock));
	}

	public void FullyUnblockCell(Vector2Int coordinates)
	{
		_modifications.Enqueue(new Modification(added: false, coordinates.XYZ(), ChangeType.FullCellBlock));
	}

	private void CreateColumns()
	{
		_columnCounts = _tickOnlyArrayService.Create<byte>(_mapIndexService.MaxIndex);
		_waterColumns = _tickOnlyArrayService.Create<WaterColumn>(_verticalStride);
		for (int i = 0; i < _mapIndexService.MaxIndex; i++)
		{
			_waterColumns.GetSpan()[i] = new WaterColumn(0, _maxColumnHeight);
			_columnCounts.GetSpan()[i] = 1;
			if (!_mapIndexService.IndexIsInActualMap(i))
			{
				continue;
			}
			for (int j = 0; j < _mapIndexService.TerrainSize.z; j++)
			{
				int index = j * _verticalStride + i;
				if (_terrainService.UnsafeCellIsTerrain(index))
				{
					AddFullObstacleInternal(i, j);
				}
			}
		}
	}

	private void Update()
	{
		AnyColumnChanged = false;
		ProcessModifications();
		if (_simulationController.ShouldResetSimulation)
		{
			Reset();
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		for (int i = change.From; i <= change.To; i++)
		{
			_modifications.Enqueue(new Modification(change.SetTerrain, change.Coordinates.ToVector3Int(i), ChangeType.FullObstacle));
		}
	}

	private void ProcessModifications()
	{
		while (!_modifications.IsEmpty())
		{
			Modification modification = _modifications.Dequeue();
			if (modification.Added)
			{
				Add(modification);
			}
			else
			{
				Remove(modification);
			}
		}
	}

	private void Add(Modification modification)
	{
		Vector3Int coordinates = modification.Coordinates;
		switch (modification.ChangeType)
		{
		case ChangeType.FullObstacle:
			AddFullObstacleInternal(_mapIndexService.CellToIndex(coordinates.XY()), coordinates.z);
			break;
		case ChangeType.HorizontalObstacle:
			AddHorizontalObstacleInternal(coordinates);
			break;
		case ChangeType.FullCellBlock:
			FullyBlockCellInternal(coordinates.XY());
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void Remove(Modification modification)
	{
		Vector3Int coordinates = modification.Coordinates;
		switch (modification.ChangeType)
		{
		case ChangeType.FullObstacle:
			RemoveFullObstacleInternal(coordinates);
			break;
		case ChangeType.HorizontalObstacle:
			RemoveHorizontalObstacleInternal(coordinates);
			break;
		case ChangeType.FullCellBlock:
			FullyUnblockCellInternal(coordinates.XY());
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void AddFullObstacleInternal(int index2D, int height)
	{
		ref WaterColumn column = ref _mutableWaterColumnRetriever.GetColumn(_columnCounts.GetReadOnlySpan(), _waterColumns.GetSpan(), _verticalStride, index2D, height);
		if (column.Floor == height)
		{
			if (column.Ceiling - 1 == height)
			{
				RemoveColumn(index2D, GetColumnIndex(index2D, height));
			}
			else
			{
				column.Floor = Convert.ToByte(column.Floor + 1);
				column.WaterDepth = ((column.WaterDepth > 1f) ? (column.WaterDepth - 1f) : 0f);
			}
		}
		else if (column.Ceiling - 1 == height)
		{
			column.Ceiling = Convert.ToByte(column.Ceiling - 1);
		}
		else
		{
			SplitColumn(ref column, index2D, height, height + 1);
		}
		AnyColumnChanged = true;
	}

	private void RemoveFullObstacleInternal(Vector3Int coordinates)
	{
		int num = _mapIndexService.CellToIndex(coordinates.XY());
		int z = coordinates.z;
		int columnIndex = GetColumnIndex(num, z + 1);
		int columnIndex2 = GetColumnIndex(num, z - 1);
		bool flag = _horizontalObstacles.GetSpan()[(z + 1) * _verticalStride + num] > 0;
		bool flag2 = _horizontalObstacles.GetSpan()[z * _verticalStride + num] > 0;
		if ((columnIndex == -1 && columnIndex2 == -1) || (flag & flag2))
		{
			InsertNewColumn(z, num);
		}
		else if (columnIndex != -1 && columnIndex2 != -1 && !flag && !flag2)
		{
			MergeColumns(num, columnIndex, columnIndex2);
		}
		else if (columnIndex != -1 && !flag)
		{
			_waterColumns.GetSpan()[columnIndex * _verticalStride + num].Floor = Convert.ToByte(z);
		}
		else if (columnIndex2 != -1 && !flag2)
		{
			_waterColumns.GetSpan()[columnIndex2 * _verticalStride + num].Ceiling = Convert.ToByte(z + 1);
		}
		else
		{
			InsertNewColumn(z, num);
		}
		AnyColumnChanged = true;
	}

	private void AddHorizontalObstacleInternal(Vector3Int coordinates)
	{
		int num = _mapIndexService.CellToIndex(coordinates.XY());
		int z = coordinates.z;
		int index = z * _verticalStride + num;
		if (++_horizontalObstacles.GetSpan()[index] != 1)
		{
			return;
		}
		int columnIndex = GetColumnIndex(num, z);
		if (columnIndex != -1)
		{
			ref WaterColumn reference = ref _waterColumns.GetSpan()[columnIndex * _verticalStride + num];
			if (reference.Floor != z)
			{
				SplitColumn(ref reference, num, z, z);
				AnyColumnChanged = true;
			}
		}
	}

	private void RemoveHorizontalObstacleInternal(Vector3Int coordinates)
	{
		int num = _mapIndexService.CellToIndex(coordinates.XY());
		int z = coordinates.z;
		int index = z * _verticalStride + num;
		if (--_horizontalObstacles.GetSpan()[index] == 0)
		{
			int columnIndex = GetColumnIndex(num, z);
			int columnIndex2 = GetColumnIndex(num, z - 1);
			if (columnIndex != -1 && columnIndex2 != -1)
			{
				MergeColumns(num, columnIndex, columnIndex2);
				AnyColumnChanged = true;
			}
		}
	}

	private void FullyBlockCellInternal(Vector2Int coordinates)
	{
		int index = _mapIndexService.CellToIndex(coordinates);
		ClearColumns(index);
		_waterColumns.GetSpan()[index] = new WaterColumn(_maxColumnHeight, _maxColumnHeight);
		_columnCounts.GetSpan()[index] = 1;
	}

	private void FullyUnblockCellInternal(Vector2Int coordinates)
	{
		int index = _mapIndexService.CellToIndex(coordinates);
		ClearColumns(index);
		_waterColumns.GetSpan()[index] = new WaterColumn(0, _maxColumnHeight);
		_columnCounts.GetSpan()[index] = 1;
	}

	private void SplitColumn(ref WaterColumn column, int index, int splitHeight, int newFloorHeight)
	{
		WaterColumn newColumn = new WaterColumn(newFloorHeight, column.Ceiling);
		column.Ceiling = Convert.ToByte(splitHeight);
		float num = (float)(int)column.Floor + column.WaterDepth;
		if (num > (float)newFloorHeight)
		{
			newColumn.WaterDepth = num - (float)newFloorHeight;
			newColumn.Contamination = column.Contamination;
			column.WaterDepth = splitHeight - column.Floor;
		}
		InsertColumn(index, splitHeight, ref newColumn);
	}

	private void MergeColumns(int index, int columnAboveIndex, int columnBelowIndex)
	{
		ref WaterColumn reference = ref _waterColumns.GetSpan()[columnAboveIndex * _verticalStride + index];
		ref WaterColumn reference2 = ref _waterColumns.GetSpan()[columnBelowIndex * _verticalStride + index];
		reference2.Ceiling = reference.Ceiling;
		float num = reference2.WaterDepth + reference.WaterDepth;
		if (num > 0f)
		{
			float num2 = reference2.WaterDepth * reference2.Contamination + reference.WaterDepth * reference.Contamination;
			reference2.Contamination = num2 / num;
		}
		reference2.WaterDepth = num;
		RemoveColumn(index, columnAboveIndex);
	}

	private void InsertNewColumn(int height, int index)
	{
		WaterColumn newColumn = new WaterColumn(height, height + 1);
		InsertColumn(index, height, ref newColumn);
	}

	private void InsertColumn(int index, int splitHeight, ref WaterColumn newColumn)
	{
		byte b = _columnCounts.GetSpan()[index];
		for (int num = _columnCounts.GetSpan()[index] - 1; num >= 0; num--)
		{
			int num2 = num * _verticalStride + index;
			ref WaterColumn reference = ref _waterColumns.GetSpan()[num2];
			if (reference.Floor <= splitHeight)
			{
				break;
			}
			int num3 = num + 1;
			if (num3 == MaxColumnCount)
			{
				IncreaseMaxColumnCount();
			}
			int num4 = num3 * _verticalStride + index;
			_waterColumns.GetSpan()[num4] = reference;
			_outflows.GetSpan()[num4] = _outflows.GetSpan()[num2];
			UpdateNeighborsOutflows(index, num2, num4);
			b = (byte)num;
		}
		int index2 = b * _verticalStride + index;
		if (_columnCounts.GetSpan()[index]++ == MaxColumnCount)
		{
			IncreaseMaxColumnCount();
		}
		_waterColumns.GetSpan()[index2] = newColumn;
		ClearOutflows(ref _outflows.GetSpan()[index2]);
	}

	private void IncreaseMaxColumnCount()
	{
		MaxColumnCount++;
		Resize();
	}

	private void Resize()
	{
		int num = MaxColumnCount * _mapIndexService.VerticalStride;
		_waterColumns.Resize(num);
		_outflows.Resize(num);
		_contaminationsBuffer.Resize(num);
		_targetedDiffusionCount.Resize(num);
		ClearOutflows((MaxColumnCount - 1) * _mapIndexService.VerticalStride, num - 1);
	}

	private void RemoveColumn(int index, int columnIndex)
	{
		byte b = _columnCounts.GetSpan()[index];
		UpdateNeighborsOutflows(index, columnIndex * _verticalStride + index, -1);
		for (int i = columnIndex + 1; i < b; i++)
		{
			int num = i * _verticalStride + index;
			int num2 = (i - 1) * _verticalStride + index;
			_waterColumns.GetSpan()[num2] = _waterColumns.GetSpan()[num];
			_outflows.GetSpan()[num2] = _outflows.GetSpan()[num];
			UpdateNeighborsOutflows(index, num, num2);
		}
		_columnCounts.GetSpan()[index]--;
		int index2 = (b - 1) * _verticalStride + index;
		_waterColumns.GetSpan()[index2] = default(WaterColumn);
		ClearOutflows(ref _outflows.GetSpan()[index2]);
	}

	private int GetColumnIndex(int index, int height)
	{
		for (int i = 0; i < _columnCounts.GetSpan()[index]; i++)
		{
			ref WaterColumn reference = ref _waterColumns.GetSpan()[i * _verticalStride + index];
			if (height >= reference.Floor && height < reference.Ceiling)
			{
				return i;
			}
		}
		return -1;
	}

	private void UpdateNeighborsOutflows(int index, int previousIndex3D, int currentIndex3D)
	{
		UpdateOutflows(index - _stride, previousIndex3D, currentIndex3D);
		UpdateOutflows(index - 1, previousIndex3D, currentIndex3D);
		UpdateOutflows(index + _stride, previousIndex3D, currentIndex3D);
		UpdateOutflows(index + 1, previousIndex3D, currentIndex3D);
	}

	private void UpdateOutflows(int index, int previousIndex3D, int currentIndex3D)
	{
		for (int i = 0; i < _columnCounts.GetSpan()[index]; i++)
		{
			ref ColumnOutflows reference = ref _outflows.GetSpan()[i * _verticalStride + index];
			if (reference.BottomFlow.Index3D == previousIndex3D)
			{
				reference.BottomFlow.Index3D = currentIndex3D;
			}
			if (reference.LeftFlow.Index3D == previousIndex3D)
			{
				reference.LeftFlow.Index3D = currentIndex3D;
			}
			if (reference.TopFlow.Index3D == previousIndex3D)
			{
				reference.TopFlow.Index3D = currentIndex3D;
			}
			if (reference.RightFlow.Index3D == previousIndex3D)
			{
				reference.RightFlow.Index3D = currentIndex3D;
			}
			if (reference.Outflows == null)
			{
				continue;
			}
			for (int j = 0; j < reference.Outflows.Count; j++)
			{
				if (reference.Outflows[j].Index3D == previousIndex3D)
				{
					reference.Outflows[j] = new TargetedFlow(reference.Outflows[j].Flow, currentIndex3D);
				}
			}
		}
	}

	private void ClearColumns(int index)
	{
		for (int i = 0; i < _columnCounts.GetSpan()[index]; i++)
		{
			int index2 = i * _verticalStride + index;
			_waterColumns.GetSpan()[index2] = default(WaterColumn);
			ClearOutflows(ref _outflows.GetSpan()[index2]);
			_columnCounts.GetSpan()[index] = 0;
		}
		AnyColumnChanged = true;
	}

	private void ClearOutflows(int firstIndex, int lastIndex)
	{
		for (int i = firstIndex; i <= lastIndex; i++)
		{
			ClearOutflows(ref _outflows.GetSpan()[i]);
		}
	}

	private void Reset()
	{
		foreach (int item in _mapIndexService.Indices2D)
		{
			for (int i = 0; i < _columnCounts.GetSpan()[item]; i++)
			{
				int index = i * _verticalStride + item;
				_waterColumns.GetSpan()[index].Reset();
				ClearOutflows(ref _outflows.GetSpan()[index]);
			}
		}
	}

	private static void ClearOutflows(ref ColumnOutflows outflows)
	{
		outflows.BottomFlow.Flow = 0f;
		outflows.BottomFlow.Index3D = -1;
		outflows.LeftFlow.Flow = 0f;
		outflows.LeftFlow.Index3D = -1;
		outflows.TopFlow.Flow = 0f;
		outflows.TopFlow.Index3D = -1;
		outflows.RightFlow.Flow = 0f;
		outflows.RightFlow.Index3D = -1;
		outflows.Outflows?.Clear();
	}
}
