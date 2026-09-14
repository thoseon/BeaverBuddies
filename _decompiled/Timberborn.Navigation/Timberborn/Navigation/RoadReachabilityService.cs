using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class RoadReachabilityService
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

	private readonly RoadNavMeshGraph _roadNavMeshGraph;

	private readonly Queue<NodeToVisit> _nodesToVisit = new Queue<NodeToVisit>();

	private readonly HashSet<int> _visitedNodes = new HashSet<int>();

	public RoadReachabilityService(RoadNavMeshGraph roadNavMeshGraph)
	{
		_roadNavMeshGraph = roadNavMeshGraph;
	}

	public void GetReachableNeighborsInRange(int startingNodeId, int range, List<int> reachableRoadNodes)
	{
		VisitNode(startingNodeId, 0f);
		while (!_nodesToVisit.IsEmpty())
		{
			NodeToVisit nodeToVisit = _nodesToVisit.Dequeue();
			if (nodeToVisit.Distance < (float)range)
			{
				reachableRoadNodes.Add(nodeToVisit.Id);
				VisitNeighbors(nodeToVisit);
			}
		}
		_visitedNodes.Clear();
	}

	private void VisitNeighbors(NodeToVisit nodeToVisit)
	{
		ReadOnlyList<NavMeshNode> neighbors = _roadNavMeshGraph.GetNeighbors(nodeToVisit.Id);
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
		_nodesToVisit.Enqueue(new NodeToVisit(nodeId, distance));
		_visitedNodes.Add(nodeId);
	}
}
