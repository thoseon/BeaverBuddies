using System.Collections.Generic;
using Timberborn.MapStateSystem;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal class WaterMapBoundaryService
{
	private readonly MapSize _mapSize;

	private readonly WaterSimulator _waterSimulator;

	private readonly List<Vector2Int> _blockedCells = new List<Vector2Int>();

	public WaterMapBoundaryService(MapSize mapSize, WaterSimulator waterSimulator)
	{
		_mapSize = mapSize;
		_waterSimulator = waterSimulator;
	}

	public void FullyBlockCell(Vector2Int coordinates)
	{
		if (!_mapSize.ContainsInTerrain(coordinates))
		{
			if (!_blockedCells.Contains(coordinates))
			{
				_waterSimulator.FullyBlockCell(coordinates);
			}
			_blockedCells.Add(coordinates);
		}
	}

	public void FullyUnblockCell(Vector2Int coordinates)
	{
		if (!_mapSize.ContainsInTerrain(coordinates))
		{
			_blockedCells.Remove(coordinates);
			if (!_blockedCells.Contains(coordinates))
			{
				_waterSimulator.FullyUnblockCell(coordinates);
			}
		}
	}
}
