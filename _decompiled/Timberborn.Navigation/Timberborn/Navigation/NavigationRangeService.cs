using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.Navigation;

internal class NavigationRangeService : INavigationRangeService
{
	private readonly NodeIdService _nodeIdService;

	private readonly NavigationDistance _navigationDistance;

	private readonly TerrainNavigationRangeService _terrainNavigationRangeService;

	private readonly RoadNavigationRangeService _roadNavigationRangeService;

	private readonly RoadSpillNavigationRangeService _roadSpillNavigationRangeService;

	public NavigationRangeService(NodeIdService nodeIdService, NavigationDistance navigationDistance, TerrainNavigationRangeService terrainNavigationRangeService, RoadNavigationRangeService roadNavigationRangeService, RoadSpillNavigationRangeService roadSpillNavigationRangeService)
	{
		_terrainNavigationRangeService = terrainNavigationRangeService;
		_nodeIdService = nodeIdService;
		_navigationDistance = navigationDistance;
		_roadNavigationRangeService = roadNavigationRangeService;
		_roadSpillNavigationRangeService = roadSpillNavigationRangeService;
	}

	public IEnumerable<WeightedCoordinates> GetRoadNodesInRange(Vector3 position)
	{
		if (!_nodeIdService.Contains(position))
		{
			return Enumerable.Empty<WeightedCoordinates>();
		}
		return _roadNavigationRangeService.GetNodesInRange(position);
	}

	public IEnumerable<WeightedCoordinates> GetRoadPreviewNodesInRange(Vector3 position)
	{
		if (!_nodeIdService.Contains(position))
		{
			return Enumerable.Empty<WeightedCoordinates>();
		}
		return _roadNavigationRangeService.GetPreviewNodesInRange(position);
	}

	public IEnumerable<Vector3Int> GetTerrainNodesInRange(Vector3 position)
	{
		if (!_nodeIdService.Contains(position))
		{
			return Enumerable.Empty<Vector3Int>();
		}
		return _terrainNavigationRangeService.GetNodesInRange(position, _navigationDistance.ResourceBuildings);
	}

	public IEnumerable<Vector3Int> GetTerrainPreviewNodesInRange(Vector3 position)
	{
		if (!_nodeIdService.Contains(position))
		{
			return Enumerable.Empty<Vector3Int>();
		}
		return _terrainNavigationRangeService.GetPreviewNodesInRange(position, _navigationDistance.ResourceBuildings);
	}

	public IEnumerable<Vector3Int> GetRoadSpillNodesInRange(Vector3 position)
	{
		if (!_nodeIdService.Contains(position))
		{
			return Enumerable.Empty<Vector3Int>();
		}
		return _roadSpillNavigationRangeService.GetNodesInRange(position);
	}

	public IEnumerable<Vector3Int> GetRoadSpillPreviewNodesInRange(Vector3 position)
	{
		if (!_nodeIdService.Contains(position))
		{
			return Enumerable.Empty<Vector3Int>();
		}
		return _roadSpillNavigationRangeService.GetPreviewNodesInRange(position);
	}
}
