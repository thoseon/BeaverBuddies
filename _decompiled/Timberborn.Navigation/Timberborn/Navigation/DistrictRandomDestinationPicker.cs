using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Navigation;

internal class DistrictRandomDestinationPicker
{
	private static readonly int RoadMaxDistance = 10;

	private static readonly int TerrainMaxDistance = 5;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly RoadReachabilityService _roadReachabilityService;

	private readonly TerrainReachabilityService _terrainReachabilityService;

	private readonly DistrictMap _districtMap;

	private readonly NodeIdService _nodeIdService;

	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly List<int> _reachableNodes = new List<int>();

	private readonly List<int> _validNodes = new List<int>();

	public DistrictRandomDestinationPicker(IRandomNumberGenerator randomNumberGenerator, RoadReachabilityService roadReachabilityService, TerrainReachabilityService terrainReachabilityService, DistrictMap districtMap, NodeIdService nodeIdService, TerrainNavMeshGraph terrainNavMeshGraph)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_roadReachabilityService = roadReachabilityService;
		_terrainReachabilityService = terrainReachabilityService;
		_districtMap = districtMap;
		_nodeIdService = nodeIdService;
		_terrainNavMeshGraph = terrainNavMeshGraph;
	}

	public Vector3 GetRandomDestination(District district, Vector3 coordinates)
	{
		int nodeId = _nodeIdService.WorldToId(coordinates);
		if (!_districtMap.TryGetParentRoadNode(district, nodeId, out var parentNode))
		{
			return coordinates;
		}
		return GetRandomDestination(parentNode);
	}

	private Vector3 GetRandomDestination(int roadNode)
	{
		_roadReachabilityService.GetReachableNeighborsInRange(roadNode, RoadMaxDistance, _reachableNodes);
		ValidateAndClearReachableNodes();
		int listElement = _randomNumberGenerator.GetListElement(_validNodes);
		_validNodes.Clear();
		_terrainReachabilityService.GetReachableNeighborsInRange(listElement, TerrainMaxDistance, _reachableNodes);
		ValidateAndClearReachableNodes();
		int nodeId = (_randomNumberGenerator.TryGetListElement(_validNodes, out var randomElement) ? randomElement : listElement);
		_validNodes.Clear();
		return _nodeIdService.IdToWorld(nodeId);
	}

	private void ValidateAndClearReachableNodes()
	{
		foreach (int reachableNode in _reachableNodes)
		{
			if (_terrainNavMeshGraph.IsConnectedToDefaultGroup(reachableNode))
			{
				_validNodes.Add(reachableNode);
			}
		}
		_reachableNodes.Clear();
	}
}
