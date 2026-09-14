using UnityEngine;

namespace Timberborn.Navigation;

public interface INavigationCachingService
{
	void StartCachingRoadFlowField(Vector3Int coordinates);

	void StopCachingRoadFlowField(Vector3Int coordinates);

	void StartCachingTerrainFlowField(Vector3Int coordinates);

	void StopCachingTerrainFlowField(Vector3Int coordinates);
}
