using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.GridTraversing;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainQueryingSystem;

public class TerrainAreaService
{
	private readonly ITerrainService _terrainService;

	private readonly TerrainPicker _terrainPicker;

	public TerrainAreaService(ITerrainService terrainService, TerrainPicker terrainPicker)
	{
		_terrainService = terrainService;
		_terrainPicker = terrainPicker;
	}

	public IEnumerable<Vector3Int> InMapCoordinates(IEnumerable<Vector2Int> blocks)
	{
		foreach (Vector2Int block in blocks)
		{
			foreach (Vector3Int item in _terrainService.GetAllHeightsInCell(block))
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Vector3Int> InMapLeveledCoordinates(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		int startHeight = (traversedCoordinates.HasValue ? (traversedCoordinates.GetValueOrDefault().Coordinates.z + 1) : 0);
		foreach (Vector3Int inputBlock in inputBlocks)
		{
			if (_terrainService.OnGround(inputBlock.Above()))
			{
				yield return new Vector3Int(inputBlock.x, inputBlock.y, startHeight);
			}
		}
	}
}
