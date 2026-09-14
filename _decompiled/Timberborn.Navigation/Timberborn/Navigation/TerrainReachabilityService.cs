using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class TerrainReachabilityService
{
	private readonly struct NodeToVisit
	{
		public int Id { get; }

		public float Distance { get; }

		public NodeToVisit(int id, float distance)
		{
			Id = id;
			Distance = distance;
		}
	}

	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private readonly RestrictedNodeMap _restrictedNodeMap;

	private readonly Queue<NodeToVisit> _nodesToVisit = new Queue<NodeToVisit>();

	private readonly HashSet<int> _visitedNodes = new HashSet<int>();

	public TerrainReachabilityService(TerrainNavMeshGraph terrainNavMeshGraph, RestrictedNodeMap restrictedNodeMap)
	{
		_terrainNavMeshGraph = terrainNavMeshGraph;
		_restrictedNodeMap = restrictedNodeMap;
	}

	public void GetReachableNeighborsInRange(int startingNodeId, int range, List<int> reachableRoadNodes)
	{
		VisitNode(startingNodeId, 0f);
		TraverseNodes(range);
		CopyNodes(reachableRoadNodes);
	}

	private void TraverseNodes(int range)
	{
		while (!_nodesToVisit.IsEmpty())
		{
			NodeToVisit nodeToVisit = _nodesToVisit.Dequeue();
			if (nodeToVisit.Distance < (float)range)
			{
				VisitNeighbors(nodeToVisit);
			}
		}
	}

	private void CopyNodes(List<int> reachableRoadNodes)
	{
		foreach (int visitedNode in _visitedNodes)
		{
			if (!_restrictedNodeMap.IsNodeRestricted(visitedNode))
			{
				reachableRoadNodes.Add(visitedNode);
			}
		}
		_visitedNodes.Clear();
	}

	private void VisitNeighbors(NodeToVisit nodeToVisit)
	{
		ReadOnlyList<NavMeshNode> neighbors = _terrainNavMeshGraph.GetNeighbors(nodeToVisit.Id);
		for (int i = 0; i < neighbors.Count; i++)
		{
			NavMeshNode navMeshNode = neighbors[i];
			int id = navMeshNode.Id;
			if (!_visitedNodes.Contains(id))
			{
				VisitNode(id, nodeToVisit.Distance + navMeshNode.Cost);
			}
		}
	}

	private void VisitNode(int nodeId, float distance)
	{
		_visitedNodes.Add(nodeId);
		_nodesToVisit.Enqueue(new NodeToVisit(nodeId, distance));
	}
}
