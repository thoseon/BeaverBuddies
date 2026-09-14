using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Navigation;

internal class GlobalReachabilityService : ILoadableSingleton, IPrioritizedSingletonInstantNavMeshListener
{
	private readonly NodeIdService _nodeIdService;

	private readonly InstantTerrainNavMeshGraph _instantTerrainNavMeshGraph;

	private readonly Dictionary<int, int> _nodesWithAssignedArea = new Dictionary<int, int>();

	private readonly Queue<int> _nodesToVisit = new Queue<int>();

	private int _areaCounter;

	private bool[] _visitedNodes;

	public GlobalReachabilityService(NodeIdService nodeIdService, InstantTerrainNavMeshGraph instantTerrainNavMeshGraph)
	{
		_nodeIdService = nodeIdService;
		_instantTerrainNavMeshGraph = instantTerrainNavMeshGraph;
	}

	public void Load()
	{
		_visitedNodes = new bool[_nodeIdService.NumberOfNodes];
	}

	public bool AreaReachable(Vector3 a, int b)
	{
		if (_nodeIdService.Contains(a))
		{
			return AreaReachable(_nodeIdService.WorldToId(a), b);
		}
		return false;
	}

	public bool AreaReachable(Vector3 a, Vector3 b)
	{
		if (_nodeIdService.Contains(a) && _nodeIdService.Contains(b))
		{
			return AreaReachable(_nodeIdService.WorldToId(a), _nodeIdService.WorldToId(b));
		}
		return false;
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		ClearAreas();
	}

	private bool AreaReachable(int aNodeId, int bNodeId)
	{
		return GetAreaOfNode(aNodeId) == GetAreaOfNode(bNodeId);
	}

	private int GetAreaOfNode(int nodeId)
	{
		if (!_nodesWithAssignedArea.TryGetValue(nodeId, out var value))
		{
			CreateAreaReachableFromNode(nodeId, _areaCounter);
			return _areaCounter++;
		}
		return value;
	}

	private void ClearAreas()
	{
		_areaCounter = 0;
		_nodesWithAssignedArea.Clear();
	}

	private void CreateAreaReachableFromNode(int startingNodeId, int area)
	{
		Array.Clear(_visitedNodes, 0, _visitedNodes.Length);
		VisitNode(startingNodeId, area);
		while (!_nodesToVisit.IsEmpty())
		{
			VisitNeighbors(_nodesToVisit.Dequeue(), area);
		}
	}

	private void VisitNeighbors(int nodeId, int area)
	{
		ReadOnlyList<NavMeshNode> neighbors = _instantTerrainNavMeshGraph.GetNeighbors(nodeId);
		for (int i = 0; i < neighbors.Count; i++)
		{
			int id = neighbors[i].Id;
			if (!_visitedNodes[id])
			{
				VisitNode(id, area);
			}
		}
	}

	private void VisitNode(int nodeId, int area)
	{
		_visitedNodes[nodeId] = true;
		_nodesToVisit.Enqueue(nodeId);
		_nodesWithAssignedArea[nodeId] = area;
	}
}
