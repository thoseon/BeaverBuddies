using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

public class DummyNavigationService : INavigationService
{
	public float HeuristicDistance(Vector3 start, Vector3 end)
	{
		return 0f;
	}

	public bool DestinationIsReachable(Vector3 start, Vector3 end)
	{
		return false;
	}

	public bool DestinationIsReachableUnlimitedRange(Vector3 start, Vector3 end)
	{
		return false;
	}

	public bool FindRoadPath(Vector3 start, Vector3 end, out float distance)
	{
		distance = 0f;
		return false;
	}

	public bool FindInstantRoadPath(Vector3 start, Vector3 end, out float distance)
	{
		distance = 0f;
		return false;
	}

	public bool FindTerrainPath(Vector3 start, Vector3 end, out float distance)
	{
		distance = 0f;
		return false;
	}

	public bool FindPathUnlimitedRange(Vector3 start, Vector3 end, List<PathCorner> pathCorners, out float distance)
	{
		distance = 0f;
		return false;
	}

	public bool FindPathUnlimitedRange(Vector3 start, IReadOnlyList<Vector3> ends, List<PathCorner> pathCorners, out float distance)
	{
		distance = 0f;
		return false;
	}

	public bool FindRoadSpillOrTerrainPathUnlimitedRange(Vector3 start, IReadOnlyList<Vector3> ends, List<PathCorner> pathCorners, out float distance)
	{
		distance = 0f;
		return false;
	}

	public bool FindPath(Vector3 start, Vector3 end, List<PathCorner> pathCorners)
	{
		return false;
	}

	public bool FindRoadToTerrainPath(Vector3 roadStart, Vector3 terrainEnd, out Vector3 endOfRoad, out float distanceFromClosestRoad, out float totalDistance)
	{
		endOfRoad = default(Vector3);
		distanceFromClosestRoad = 0f;
		totalDistance = 0f;
		return false;
	}

	public bool InStoppingProximity(Vector3 a, Vector3 b)
	{
		return false;
	}

	public bool IsOnNavMesh(Vector3 position)
	{
		return false;
	}

	public Vector3? ClosestPositionOnNavMesh(Vector3 position, float maxDistance)
	{
		return null;
	}
}
