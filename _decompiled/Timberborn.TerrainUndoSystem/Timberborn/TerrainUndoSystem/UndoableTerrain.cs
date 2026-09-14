using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainUndoSystem;

internal class UndoableTerrain
{
	private readonly ITerrainService _terrainService;

	private readonly TerrainHeightChange _terrainHeightChange;

	public UndoableTerrain(ITerrainService terrainService, TerrainHeightChange terrainHeightChange)
	{
		_terrainService = terrainService;
		_terrainHeightChange = terrainHeightChange;
	}

	public void SetTerrain()
	{
		Vector3Int coordinates = new Vector3Int(_terrainHeightChange.Coordinates.x, _terrainHeightChange.Coordinates.y, _terrainHeightChange.From);
		_terrainService.SetTerrain(coordinates, _terrainHeightChange.To - _terrainHeightChange.From + 1);
	}

	public void UnsetTerrain()
	{
		Vector3Int coordinates = new Vector3Int(_terrainHeightChange.Coordinates.x, _terrainHeightChange.Coordinates.y, _terrainHeightChange.To);
		_terrainService.UnsetTerrain(coordinates, _terrainHeightChange.To - _terrainHeightChange.From + 1);
	}
}
