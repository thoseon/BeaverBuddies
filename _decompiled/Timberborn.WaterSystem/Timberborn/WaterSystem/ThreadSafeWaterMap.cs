using System;
using System.Runtime.InteropServices;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.WaterSystem;

[MapEditorTickable]
internal class ThreadSafeWaterMap : ILoadableSingleton, IPostLoadableSingleton, IThreadSafeWaterMap, ITickableSingleton
{
	private readonly MapIndexService _mapIndexService;

	private readonly ITerrainService _terrainService;

	private readonly WaterColumnRetriever _waterColumnRetriever;

	private readonly FlowVectorCalculator _flowVectorCalculator;

	private readonly WaterSimulator _waterSimulator;

	private byte[] _threadSafeColumnCounts;

	private ReadOnlyWaterColumn[] _threadSafeWaterColumns;

	private Vector2[] _waterFlowDirections;

	private int _verticalStride;

	public int MaxColumnCount { get; private set; }

	public bool AnyColumnChanged { get; private set; }

	public ReadOnlyArray<byte> ColumnCounts => new ReadOnlyArray<byte>(_threadSafeColumnCounts);

	public ReadOnlyArray<ReadOnlyWaterColumn> WaterColumns => new ReadOnlyArray<ReadOnlyWaterColumn>(_threadSafeWaterColumns);

	public ReadOnlyArray<Vector2> FlowDirections => new ReadOnlyArray<Vector2>(_waterFlowDirections);

	public event EventHandler<int> MaxWaterColumnCountChanged;

	public ThreadSafeWaterMap(MapIndexService mapIndexService, ITerrainService terrainService, WaterColumnRetriever waterColumnRetriever, FlowVectorCalculator flowVectorCalculator, WaterSimulator waterSimulator)
	{
		_mapIndexService = mapIndexService;
		_terrainService = terrainService;
		_waterColumnRetriever = waterColumnRetriever;
		_flowVectorCalculator = flowVectorCalculator;
		_waterSimulator = waterSimulator;
	}

	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
		_threadSafeColumnCounts = new byte[_mapIndexService.MaxIndex];
		_threadSafeWaterColumns = new ReadOnlyWaterColumn[_verticalStride];
		_waterFlowDirections = new Vector2[_verticalStride];
	}

	public void PostLoad()
	{
		Update();
	}

	public void Tick()
	{
		Update();
	}

	public int ColumnCount(int index2D)
	{
		return _threadSafeColumnCounts[index2D];
	}

	public byte ColumnFloor(int index3D)
	{
		return _threadSafeWaterColumns[index3D].Floor;
	}

	public byte ColumnCeiling(int index3D)
	{
		return _threadSafeWaterColumns[index3D].Ceiling;
	}

	public byte ColumnCeiling(Vector3Int coordinates)
	{
		return GetColumn(coordinates).Ceiling;
	}

	public float WaterDepth(int index3D)
	{
		return _threadSafeWaterColumns[index3D].WaterDepth;
	}

	public bool IsWaterOnAnyHeight(Vector2Int coordinates)
	{
		if (_terrainService.Contains(coordinates))
		{
			int num = _mapIndexService.CellToIndex(coordinates);
			for (int i = 0; i < _threadSafeColumnCounts[num]; i++)
			{
				int num2 = i * _verticalStride + num;
				if (_threadSafeWaterColumns[num2].WaterDepth > 0f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool TryGetColumnFloor(Vector3Int coordinates, out int floor)
	{
		if (_terrainService.Contains(coordinates.XY()))
		{
			int num = _mapIndexService.CellToIndex(coordinates.XY());
			for (int i = 0; i < _threadSafeColumnCounts[num]; i++)
			{
				int num2 = i * _verticalStride + num;
				ref ReadOnlyWaterColumn reference = ref _threadSafeWaterColumns[num2];
				floor = reference.Floor;
				if (floor > coordinates.z)
				{
					break;
				}
				if (reference.Ceiling > coordinates.z)
				{
					return true;
				}
			}
		}
		floor = 0;
		return false;
	}

	public Vector2 WaterFlowDirection(Vector3Int coordinates)
	{
		int num = _mapIndexService.CellToIndex(coordinates.XY());
		int z = coordinates.z;
		for (int i = 0; i < _threadSafeColumnCounts[num]; i++)
		{
			int num2 = i * _verticalStride + num;
			ref ReadOnlyWaterColumn reference = ref _threadSafeWaterColumns[num2];
			if (z < reference.Floor)
			{
				break;
			}
			if (z < reference.Ceiling)
			{
				return _waterFlowDirections[num2];
			}
		}
		return Vector2.zero;
	}

	public float WaterDepth(Vector3Int coordinates)
	{
		return GetColumn(coordinates).WaterDepth;
	}

	public float ColumnContamination(Vector3Int coordinates)
	{
		ref readonly ReadOnlyWaterColumn column = ref GetColumn(coordinates);
		if (!((float)(int)column.Floor + column.WaterDepth > (float)coordinates.z))
		{
			return 0f;
		}
		return column.Contamination;
	}

	public float ColumnOverflow(Vector3Int coordinates)
	{
		return GetColumn(coordinates).Overflow;
	}

	public int CeiledWaterHeight(Vector3Int coordinates)
	{
		if (_terrainService.Contains(coordinates.XY()))
		{
			ref readonly ReadOnlyWaterColumn column = ref GetColumn(coordinates);
			float waterDepth = column.WaterDepth;
			if (!(waterDepth > 0f))
			{
				return 0;
			}
			return Mathf.CeilToInt((float)(int)column.Floor + waterDepth);
		}
		return 0;
	}

	public float WaterHeightOrFloor(Vector3Int coordinates)
	{
		ref readonly ReadOnlyWaterColumn column = ref GetColumn(coordinates);
		return column.WaterDepth + (float)(int)column.Floor;
	}

	public bool CellIsUnderwater(Vector3Int coordinates)
	{
		int num = CeiledWaterHeight(coordinates);
		if (num > coordinates.z)
		{
			return num > 0;
		}
		return false;
	}

	private void Update()
	{
		int maxColumnCount = MaxColumnCount;
		MaxColumnCount = _waterSimulator.MaxColumnCount;
		AnyColumnChanged = _waterSimulator.AnyColumnChanged;
		if (MaxColumnCount > maxColumnCount)
		{
			int newSize = MaxColumnCount * _mapIndexService.VerticalStride;
			Array.Resize(ref _threadSafeWaterColumns, newSize);
			Array.Resize(ref _waterFlowDirections, newSize);
		}
		MemoryMarshal.Cast<WaterColumn, ReadOnlyWaterColumn>(_waterSimulator.WaterColumns).CopyTo(_threadSafeWaterColumns);
		if (AnyColumnChanged)
		{
			_waterSimulator.ColumnCounts.CopyTo(_threadSafeColumnCounts);
		}
		if (MaxColumnCount > maxColumnCount)
		{
			MaxWaterColumnCountChanged?.Invoke(this, MaxColumnCount);
		}
		UpdateWaterFlowDirections(_waterSimulator.ColumnCounts, _waterSimulator.Outflows);
	}

	private void UpdateWaterFlowDirections(ReadOnlySpan<byte> columnCounts, ReadOnlySpan<ColumnOutflows> outflows)
	{
		foreach (int item in _mapIndexService.Indices2D)
		{
			byte b = columnCounts[item];
			for (int i = 0; i < b; i++)
			{
				int num = i * _verticalStride + item;
				ref readonly ColumnOutflows outflows2 = ref outflows[num];
				_waterFlowDirections[num] = _flowVectorCalculator.GetFlowVectorAtTop(in outflows2);
			}
		}
	}

	private ref readonly ReadOnlyWaterColumn GetColumn(Vector3Int coordinates)
	{
		int index = _mapIndexService.CellToIndex(coordinates.XY());
		return ref _waterColumnRetriever.GetColumn(ColumnCounts, WaterColumns, _verticalStride, index, coordinates.z);
	}
}
