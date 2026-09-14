using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

public interface INavigationRangeService
{
	IEnumerable<WeightedCoordinates> GetRoadNodesInRange(Vector3 position);

	IEnumerable<WeightedCoordinates> GetRoadPreviewNodesInRange(Vector3 position);

	IEnumerable<Vector3Int> GetTerrainNodesInRange(Vector3 position);

	IEnumerable<Vector3Int> GetTerrainPreviewNodesInRange(Vector3 position);

	IEnumerable<Vector3Int> GetRoadSpillNodesInRange(Vector3 position);

	IEnumerable<Vector3Int> GetRoadSpillPreviewNodesInRange(Vector3 position);
}
