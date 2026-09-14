using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.TerrainSystem;

public class TerrainCutout
{
	private readonly ITerrainService _terrainService;

	public TerrainCutout(ITerrainService terrainService)
	{
		_terrainService = terrainService;
	}

	public void SetCutout(IEnumerable<Vector3Int> positionedCutoutTiles)
	{
		foreach (Vector3Int positionedCutoutTile in positionedCutoutTiles)
		{
			_terrainService.SetCutout(positionedCutoutTile);
		}
	}

	public void UnsetCutout(IEnumerable<Vector3Int> positionedCutoutTiles)
	{
		foreach (Vector3Int positionedCutoutTile in positionedCutoutTiles)
		{
			_terrainService.UnsetCutout(positionedCutoutTile);
		}
	}
}
