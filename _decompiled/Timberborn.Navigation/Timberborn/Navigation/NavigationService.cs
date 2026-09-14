using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

internal class NavigationService : INavigationService
{
	private static readonly float StoppingDistanceSquared = 0.010000001f;

	private readonly PathfindingService _pathfindingService;

	private readonly NavMeshPositionService _navMeshPositionService;

	private readonly INavMeshService _navMeshService;

	private readonly GlobalReachabilityService _globalReachabilityService;

	private readonly NavigationDistance _navigationDistance;

	private readonly NavMeshGroupService _navMeshGroupService;

	public NavigationService(PathfindingService pathfindingService, NavMeshPositionService navMeshPositionService, INavMeshService navMeshService, GlobalReachabilityService globalReachabilityService, NavigationDistance navigationDistance, NavMeshGroupService navMeshGroupService)
	{
		_pathfindingService = pathfindingService;
		_navMeshPositionService = navMeshPositionService;
		_navMeshService = navMeshService;
		_globalReachabilityService = globalReachabilityService;
		_navigationDistance = navigationDistance;
		_navMeshGroupService = navMeshGroupService;
	}

	public float HeuristicDistance(Vector3 start, Vector3 end)
	{
		return Vector3.Distance(start, end);
	}

	public bool DestinationIsReachable(Vector3 start, Vector3 end)
	{
		return FindPathLimitedRange(start, end, null);
	}

	public bool DestinationIsReachableUnlimitedRange(Vector3 start, Vector3 end)
	{
		return _globalReachabilityService.AreaReachable(start, end);
	}

	public bool FindRoadPath(Vector3 start, Vector3 end, out float distance)
	{
		return _pathfindingService.FindRoadPathCached(start, end, out distance);
	}

	public bool FindInstantRoadPath(Vector3 start, Vector3 end, out float distance)
	{
		return _pathfindingService.FindInstantRoadPath(start, end, out distance);
	}

	public bool FindTerrainPath(Vector3 start, Vector3 end, out float distance)
	{
		return _pathfindingService.FindTerrainPathCached(start, end, _navigationDistance.ResourceBuildings, out distance);
	}

	public bool FindPath(Vector3 start, Vector3 end, List<PathCorner> pathCorners)
	{
		return FindPathLimitedRange(start, end, pathCorners);
	}

	public bool FindPathUnlimitedRange(Vector3 start, Vector3 end, List<PathCorner> pathCorners, out float distance)
	{
		if (DestinationIsReachableUnlimitedRange(start, end, out distance, pathCorners))
		{
			return _pathfindingService.FindPathUncached(start, end, out distance, pathCorners);
		}
		return false;
	}

	public bool FindPathUnlimitedRange(Vector3 start, IReadOnlyList<Vector3> ends, List<PathCorner> pathCorners, out float distance)
	{
		return FindPathUnlimitedRange(start, ends, pathCorners, findUncached: true, out distance);
	}

	public bool FindRoadSpillOrTerrainPathUnlimitedRange(Vector3 start, IReadOnlyList<Vector3> ends, List<PathCorner> pathCorners, out float distance)
	{
		return FindPathUnlimitedRange(start, ends, pathCorners, findUncached: false, out distance);
	}

	public bool FindRoadToTerrainPath(Vector3 roadStart, Vector3 terrainEnd, out Vector3 endOfRoad, out float distanceFromClosestRoad, out float totalDistance)
	{
		return _pathfindingService.FindPathFromRoadToTerrainCached(roadStart, terrainEnd, out endOfRoad, out distanceFromClosestRoad, out totalDistance);
	}

	public bool InStoppingProximity(Vector3 a, Vector3 b)
	{
		return (a - b).sqrMagnitude < StoppingDistanceSquared;
	}

	public bool IsOnNavMesh(Vector3 position)
	{
		return _navMeshService.IsOnNavMesh(NavigationCoordinateSystem.WorldToGridInt(position));
	}

	public Vector3? ClosestPositionOnNavMesh(Vector3 position, float maxDistance)
	{
		return _navMeshPositionService.ClosestPositionOnNavMesh(position, maxDistance);
	}

	private bool FindPathLimitedRange(Vector3 start, Vector3 end, List<PathCorner> pathCorners)
	{
		if (DestinationIsReachableUnlimitedRange(start, end, out var distance, pathCorners))
		{
			return _pathfindingService.FindPathUncached(start, end, out distance, pathCorners);
		}
		return false;
	}

	private bool DestinationIsReachableUnlimitedRange(Vector3 start, Vector3 end, out float distance, IList pathCorners)
	{
		distance = 0f;
		if (DestinationIsReachableUnlimitedRange(start, end))
		{
			return true;
		}
		pathCorners?.Clear();
		return false;
	}

	private bool FindPathUnlimitedRange(Vector3 start, IReadOnlyList<Vector3> ends, List<PathCorner> pathCorners, bool findUncached, out float distance)
	{
		bool flag = false;
		foreach (Vector3 end in ends)
		{
			if (InStoppingProximity(start, end))
			{
				pathCorners?.Clear();
				pathCorners?.Add(new PathCorner(end, 1f, _navMeshGroupService.GetDefaultGroupId()));
				distance = 0f;
				return true;
			}
			if (!flag && DestinationIsReachableUnlimitedRange(start, end))
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (findUncached)
			{
				return _pathfindingService.FindPathUncached(start, ends, out distance, pathCorners);
			}
			return _pathfindingService.FindRoadSpillOrTerrainPath(start, ends, out distance, pathCorners);
		}
		pathCorners?.Clear();
		distance = 0f;
		return false;
	}
}
