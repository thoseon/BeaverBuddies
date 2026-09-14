using UnityEngine;

namespace Timberborn.Navigation;

internal class NavigationCachingService : INavigationCachingService
{
	private readonly RoadFlowFieldCache _roadFlowFieldCache;

	private readonly TerrainFlowFieldCache _terrainFlowFieldCache;

	private readonly NodeIdService _nodeIdService;

	public NavigationCachingService(RoadFlowFieldCache roadFlowFieldCache, TerrainFlowFieldCache terrainFlowFieldCache, NodeIdService nodeIdService)
	{
		_roadFlowFieldCache = roadFlowFieldCache;
		_terrainFlowFieldCache = terrainFlowFieldCache;
		_nodeIdService = nodeIdService;
	}

	public void StartCachingRoadFlowField(Vector3Int coordinates)
	{
		_roadFlowFieldCache.StartCachingAtNode(_nodeIdService.GridToId(coordinates));
	}

	public void StopCachingRoadFlowField(Vector3Int coordinates)
	{
		_roadFlowFieldCache.StopCachingAtNode(_nodeIdService.GridToId(coordinates));
	}

	public void StartCachingTerrainFlowField(Vector3Int coordinates)
	{
		_terrainFlowFieldCache.StartCachingAtNode(_nodeIdService.GridToId(coordinates));
	}

	public void StopCachingTerrainFlowField(Vector3Int coordinates)
	{
		_terrainFlowFieldCache.StopCachingAtNode(_nodeIdService.GridToId(coordinates));
	}
}
