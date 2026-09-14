using UnityEngine;

namespace Timberborn.Navigation;

public class DummyNavigationCachingService : INavigationCachingService
{
	public void StartCachingRoadFlowField(Vector3Int coordinates)
	{
	}

	public void StopCachingRoadFlowField(Vector3Int coordinates)
	{
	}

	public void StartCachingTerrainFlowField(Vector3Int coordinates)
	{
	}

	public void StopCachingTerrainFlowField(Vector3Int coordinates)
	{
	}
}
