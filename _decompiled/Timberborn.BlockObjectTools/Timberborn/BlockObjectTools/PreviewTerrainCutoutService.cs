using System.Collections.Generic;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockObjectTools;

internal class PreviewTerrainCutoutService
{
	private readonly ITerrainService _terrainService;

	private readonly HashSet<Vector3Int> _cutoutTiles = new HashSet<Vector3Int>();

	public PreviewTerrainCutoutService(ITerrainService terrainService)
	{
		_terrainService = terrainService;
	}

	public void SetCutout(IEnumerable<Vector3Int> positionedCutoutTiles)
	{
		foreach (Vector3Int positionedCutoutTile in positionedCutoutTiles)
		{
			SetCutout(positionedCutoutTile);
		}
	}

	public void UnsetCutout(IEnumerable<Vector3Int> positionedCutoutTiles)
	{
		foreach (Vector3Int positionedCutoutTile in positionedCutoutTiles)
		{
			UnsetCutout(positionedCutoutTile);
		}
	}

	public void SetCutout(Vector3Int positionedCutoutTile)
	{
		if (_cutoutTiles.Add(positionedCutoutTile))
		{
			_terrainService.SetCutout(positionedCutoutTile);
		}
	}

	public void UnsetCutout(Vector3Int positionedCutoutTile)
	{
		if (_cutoutTiles.Remove(positionedCutoutTile))
		{
			_terrainService.UnsetCutout(positionedCutoutTile);
		}
	}
}
