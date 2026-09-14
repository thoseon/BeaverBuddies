using System;
using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Navigation;

internal class RoadSpillFlowFieldGenerator
{
	private readonly struct Node
	{
		public int NodeId { get; }

		public int RoadParentNodeId { get; }

		public int SimpleDistanceToRoad { get; }

		public Node(int nodeId, int roadParentNodeId, int simpleDistanceToRoad)
		{
			NodeId = nodeId;
			RoadParentNodeId = roadParentNodeId;
			SimpleDistanceToRoad = simpleDistanceToRoad;
		}
	}

	private readonly NodeIdService _nodeIdService;

	private readonly Queue<Node> _openSet = new Queue<Node>();

	private RoadSpillFlowField _flowField;

	private int _maxTerrainDistance;

	private int _doubledMaxTerrainDistance;

	public RoadSpillFlowFieldGenerator(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public void FillFlowFieldUpToDistance(TerrainNavMeshGraph terrainNavMeshGraph, AccessFlowField startRoadFlowField, int maxTerrainDistance, RoadSpillFlowField flowField)
	{
		_flowField = flowField;
		_maxTerrainDistance = maxTerrainDistance;
		_doubledMaxTerrainDistance = maxTerrainDistance * 2;
		if (!_flowField.IsFilled)
		{
			_flowField.Clear();
			_openSet.Clear();
			PushStartingNodes(startRoadFlowField);
			while (!_openSet.IsEmpty())
			{
				VisitNeighbors(terrainNavMeshGraph, _openSet.Dequeue());
			}
			_flowField.FinishFilling();
		}
	}

	private void PushStartingNodes(AccessFlowField startRoadFlowField)
	{
		foreach (FlowFieldNode allNode in startRoadFlowField.GetAllNodes())
		{
			PushNode(allNode.Id, -1, allNode.Id, 0);
		}
	}

	private void VisitNeighbors(TerrainNavMeshGraph terrainNavMeshGraph, Node node)
	{
		ReadOnlyList<int> cheapNeighbors = terrainNavMeshGraph.GetCheapNeighbors(node.NodeId);
		for (int i = 0; i < cheapNeighbors.Count; i++)
		{
			VisitNode(node, cheapNeighbors[i]);
		}
	}

	private void VisitNode(Node parentNode, int nodeId)
	{
		if (_flowField.HasNode(nodeId))
		{
			return;
		}
		int roadParentNodeId = parentNode.RoadParentNodeId;
		Vector3Int vector3Int = _nodeIdService.IdToGrid(nodeId);
		Vector3Int vector3Int2 = _nodeIdService.IdToGrid(roadParentNodeId);
		if (Math.Abs(vector3Int.x - vector3Int2.x) < _maxTerrainDistance && Math.Abs(vector3Int.y - vector3Int2.y) < _maxTerrainDistance)
		{
			int num = parentNode.SimpleDistanceToRoad + 1;
			if (num < _doubledMaxTerrainDistance)
			{
				PushNode(nodeId, parentNode.NodeId, roadParentNodeId, num);
			}
		}
	}

	private void PushNode(int nodeId, int parentNodeId, int roadParentNodeId, int distanceToRoad)
	{
		_openSet.Enqueue(new Node(nodeId, roadParentNodeId, distanceToRoad));
		_flowField.AddNode(nodeId, parentNodeId, roadParentNodeId, distanceToRoad);
	}
}
