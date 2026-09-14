using System;
using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.WaterObjects;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

internal class WaterFlowLimitUpdater : ILoadableSingleton
{
	private readonly IFlowLimiterService _flowLimiterService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly ITerrainService _terrainService;

	private readonly MapIndexService _mapIndexService;

	private readonly EventBus _eventBus;

	private Vector2Int _mapSize;

	private float[][] _flowLimits;

	private readonly HashSet<Vector2Int> _coordinatesToUpdate = new HashSet<Vector2Int>();

	public ReadOnlyJaggedArray<float> FlowLimits => new ReadOnlyJaggedArray<float>(_flowLimits);

	public WaterFlowLimitUpdater(IFlowLimiterService flowLimiterService, IThreadSafeWaterMap threadSafeWaterMap, ITerrainService terrainService, MapIndexService mapIndexService, EventBus eventBus)
	{
		_flowLimiterService = flowLimiterService;
		_threadSafeWaterMap = threadSafeWaterMap;
		_terrainService = terrainService;
		_mapIndexService = mapIndexService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_mapSize = _mapIndexService.TerrainSize.XY();
		_flowLimits = new float[1][] { new float[_mapSize.x * _mapSize.y] };
		_flowLimiterService.HeightLimitValueChanged += OnHeightLimitValueChanged;
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBlockObjectSet(BlockObjectSetEvent blockObjectSetEvent)
	{
		if (blockObjectSetEvent.BlockObject.HasComponent<WaterObstacle>())
		{
			Vector2Int item = blockObjectSetEvent.BlockObject.Coordinates.XY();
			_coordinatesToUpdate.Add(item);
		}
	}

	[OnEvent]
	public void OnBlockObjectUnset(BlockObjectUnsetEvent blockObjectUnsetEvent)
	{
		if (blockObjectUnsetEvent.BlockObject.HasComponent<WaterObstacle>())
		{
			Vector2Int item = blockObjectUnsetEvent.BlockObject.Coordinates.XY();
			_coordinatesToUpdate.Add(item);
		}
	}

	public void Resize(int maxColumnCount)
	{
		int num = _flowLimits.Length;
		Array.Resize(ref _flowLimits, maxColumnCount);
		for (int i = num; i < maxColumnCount; i++)
		{
			_flowLimits[i] = new float[_mapSize.x * _mapSize.y];
		}
	}

	public void UpdateFlowLimits()
	{
		if (_coordinatesToUpdate.Count <= 0)
		{
			return;
		}
		ReadOnlySpan<float> asSpan = _flowLimiterService.HeightLimits.AsSpan;
		foreach (Vector2Int item in _coordinatesToUpdate)
		{
			int index2D = _mapIndexService.CellToIndex(item);
			int num = _threadSafeWaterMap.ColumnCount(index2D);
			for (int i = 0; i < num; i++)
			{
				int num2 = _mapIndexService.CoordinatesToActualMapIndex(item);
				_flowLimits[i][num2] = GetFlowLimitInColumn(index2D, item, i, asSpan);
			}
		}
		_coordinatesToUpdate.Clear();
	}

	private float GetFlowLimitInColumn(int index2D, Vector2Int coordinates2D, int columnIndex, ReadOnlySpan<float> flowLimits)
	{
		int index3D = index2D + columnIndex * _mapIndexService.VerticalStride;
		byte num = _threadSafeWaterMap.ColumnFloor(index3D);
		byte b = _threadSafeWaterMap.ColumnCeiling(index3D);
		for (byte b2 = num; b2 <= b; b2++)
		{
			Vector3Int coordinates = new Vector3Int(coordinates2D.x, coordinates2D.y, b2);
			int index = index2D + b2 * _mapIndexService.VerticalStride;
			float num2 = flowLimits[index];
			if (_terrainService.Contains(coordinates) && num2 > 0f)
			{
				return num2;
			}
		}
		return 0f;
	}

	private void OnHeightLimitValueChanged(object sender, int index3D)
	{
		_coordinatesToUpdate.Add(_mapIndexService.Index3DToCoordinates(index3D).XY());
	}
}
