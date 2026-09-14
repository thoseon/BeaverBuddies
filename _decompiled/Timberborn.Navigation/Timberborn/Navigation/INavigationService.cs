using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

public interface INavigationService
{
	float HeuristicDistance(Vector3 start, Vector3 end);

	bool DestinationIsReachable(Vector3 start, Vector3 end);

	bool DestinationIsReachableUnlimitedRange(Vector3 start, Vector3 end);

	bool FindRoadPath(Vector3 start, Vector3 end, out float distance);

	bool FindInstantRoadPath(Vector3 access, Vector3 end, out float distance);

	bool FindTerrainPath(Vector3 start, Vector3 end, out float distance);

	bool FindPath(Vector3 start, Vector3 end, List<PathCorner> pathCorners);

	bool FindPathUnlimitedRange(Vector3 start, Vector3 end, List<PathCorner> pathCorners, out float distance);

	bool FindPathUnlimitedRange(Vector3 start, IReadOnlyList<Vector3> ends, List<PathCorner> pathCorners, out float distance);

	bool FindRoadSpillOrTerrainPathUnlimitedRange(Vector3 start, IReadOnlyList<Vector3> ends, List<PathCorner> pathCorners, out float distance);

	bool FindRoadToTerrainPath(Vector3 roadStart, Vector3 terrainEnd, out Vector3 endOfRoad, out float distanceFromClosestRoad, out float totalDistance);

	bool InStoppingProximity(Vector3 a, Vector3 b);

	bool IsOnNavMesh(Vector3 position);

	Vector3? ClosestPositionOnNavMesh(Vector3 position, float maxDistance);
}
