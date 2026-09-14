using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class RoadAStarPathfinder : ILoadableSingleton
{
	private readonly HeuristicsCalculator _heuristicsCalculator;

	private readonly BinaryHeapFactory _binaryHeapFactory;

	private BinaryHeap<AStarNode> _openSet;

	private readonly HashSet<int> _destinationNodes = new HashSet<int>();

	private PathFlowField _flowField;

	public RoadAStarPathfinder(HeuristicsCalculator heuristicsCalculator, BinaryHeapFactory binaryHeapFactory)
	{
		_heuristicsCalculator = heuristicsCalculator;
		_binaryHeapFactory = binaryHeapFactory;
	}

	public void Load()
	{
		_openSet = _binaryHeapFactory.Create<AStarNode>();
	}

	public void FillFlowFieldWithPath(RoadNavMeshGraph roadNavMeshGraph, PathFlowField flowField, int startNodeId, int destinationNodeId)
	{
		_flowField = flowField;
		_heuristicsCalculator.SetDestinationNode(destinationNodeId);
		if (_flowField.CheckedPath(startNodeId, destinationNodeId) || !HeuristicallyReachable(roadNavMeshGraph, startNodeId, destinationNodeId))
		{
			return;
		}
		_flowField.Clear(startNodeId);
		_openSet.Clear();
		PushStartingNode(startNodeId);
		while (!_openSet.IsEmpty())
		{
			AStarNode node = _openSet.Pop();
			int nodeId = node.NodeId;
			if (!_flowField.HasNode(nodeId))
			{
				_flowField.AddNode(nodeId, node.ParentNodeId, node.GScore);
				if (nodeId == destinationNodeId)
				{
					_flowField.MarkAsPartiallyFilled();
					return;
				}
				VisitNeighbors(roadNavMeshGraph, node);
			}
		}
		_flowField.MarkAsFullyFilled();
	}

	public bool FillFlowFieldWithPath(RoadNavMeshGraph roadNavMeshGraph, PathFlowField flowField, int startNodeId, IReadOnlyList<int> destinationNodeIds, out int destinationNodeId)
	{
		_flowField = flowField;
		_heuristicsCalculator.SetDestinationNodes(destinationNodeIds);
		_destinationNodes.Clear();
		_destinationNodes.AddRange(destinationNodeIds);
		_flowField.Clear(startNodeId);
		_openSet.Clear();
		PushStartingNode(startNodeId);
		while (!_openSet.IsEmpty())
		{
			AStarNode node = _openSet.Pop();
			int nodeId = node.NodeId;
			if (!_flowField.HasNode(nodeId))
			{
				_flowField.AddNode(nodeId, node.ParentNodeId, node.GScore);
				if (_destinationNodes.Contains(nodeId))
				{
					_flowField.MarkAsPartiallyFilled();
					destinationNodeId = nodeId;
					return true;
				}
				VisitNeighbors(roadNavMeshGraph, node);
			}
		}
		_flowField.MarkAsFullyFilled();
		destinationNodeId = 0;
		return false;
	}

	private static bool HeuristicallyReachable(RoadNavMeshGraph roadNavMeshGraph, int startNodeId, int destinationNodeId)
	{
		if (roadNavMeshGraph.IsOnNavMesh(startNodeId))
		{
			return roadNavMeshGraph.IsOnNavMesh(destinationNodeId);
		}
		return false;
	}

	private void VisitNeighbors(RoadNavMeshGraph roadNavMeshGraph, AStarNode node)
	{
		ReadOnlyList<NavMeshNode> neighbors = roadNavMeshGraph.GetNeighbors(node.NodeId);
		for (int i = 0; i < neighbors.Count; i++)
		{
			VisitNode(node, neighbors[i]);
		}
	}

	private void VisitNode(AStarNode parentNode, NavMeshNode node)
	{
		int id = node.Id;
		if (!_flowField.HasNode(id))
		{
			float gScore = parentNode.GScore + node.Cost;
			PushNode(id, parentNode.NodeId, gScore);
		}
	}

	private void PushStartingNode(int nodeId)
	{
		PushNode(nodeId, -1, 0f);
	}

	private void PushNode(int nodeId, int parentNodeId, float gScore)
	{
		float num = _heuristicsCalculator.H(nodeId);
		float fScore = gScore + num;
		_openSet.Push(new AStarNode(nodeId, parentNodeId, gScore, fScore));
	}
}
