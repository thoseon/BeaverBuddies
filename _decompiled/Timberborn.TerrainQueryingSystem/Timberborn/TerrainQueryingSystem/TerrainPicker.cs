using System;
using Timberborn.GridTraversing;
using Timberborn.LevelVisibilitySystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainQueryingSystem;

public class TerrainPicker
{
	private readonly ITerrainService _terrainService;

	private readonly GridTraversal _gridTraversal;

	private readonly ILevelVisibilityService _levelVisibilityService;

	public TerrainPicker(ITerrainService terrainService, GridTraversal gridTraversal, ILevelVisibilityService levelVisibilityService)
	{
		_terrainService = terrainService;
		_gridTraversal = gridTraversal;
		_levelVisibilityService = levelVisibilityService;
	}

	public TraversedCoordinates? PickTerrainCoordinates(Ray ray)
	{
		return PickCoordinates(ray, IsTerrainVoxel);
	}

	public TraversedCoordinates? PickTerrainCoordinatesWithStump(Ray ray)
	{
		return PickCoordinates(ray, IsTerrainVoxelIncludeTerrainStump);
	}

	public TraversedCoordinates? PickTerrainCoordinates(Ray ray, Predicate<Vector3Int> additionalStopCondition)
	{
		return PickCoordinates(ray, (Vector3Int coordinates) => IsTerrainVoxel(coordinates) || additionalStopCondition(coordinates));
	}

	public TraversedCoordinates? FindCoordinatesOnLevelInMap(Ray ray, float level)
	{
		Vector3? vector = GridSpaceRaycasting.HitHorizontalPlane(ray, level);
		if (vector.HasValue)
		{
			Vector3Int coordinates = new Vector3Int(Mathf.FloorToInt(vector.Value.x), Mathf.FloorToInt(vector.Value.y), Mathf.RoundToInt(vector.Value.z));
			return new TraversedCoordinates(_terrainService.Clamp(coordinates), new Vector3Int(0, 0, 1), vector.Value);
		}
		return null;
	}

	private TraversedCoordinates? PickCoordinates(Ray ray, Predicate<Vector3Int> predicate)
	{
		foreach (TraversedCoordinates item in _gridTraversal.TraverseRay(ray))
		{
			Vector3Int coordinates = item.Coordinates;
			if (predicate(coordinates))
			{
				return item;
			}
		}
		return null;
	}

	private bool IsTerrainVoxel(Vector3Int coordinates)
	{
		if (_terrainService.Underground(coordinates))
		{
			return coordinates.z < _levelVisibilityService.MaxVisibleLevel;
		}
		return false;
	}

	private bool IsTerrainVoxelIncludeTerrainStump(Vector3Int coordinates)
	{
		if (_terrainService.Underground(coordinates))
		{
			return coordinates.z <= _levelVisibilityService.MaxVisibleLevel;
		}
		return false;
	}
}
