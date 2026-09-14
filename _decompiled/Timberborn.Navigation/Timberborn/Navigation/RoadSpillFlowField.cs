using System.Collections.Generic;

namespace Timberborn.Navigation;

internal class RoadSpillFlowField : IFlowField
{
	private readonly struct Node
	{
		public int ParentNodeId { get; }

		public int RoadParentNodeId { get; }

		public float DistanceToRoad { get; }

		public Node(int parentNodeId, int roadParentNodeId, float distanceToRoad)
		{
			ParentNodeId = parentNodeId;
			RoadParentNodeId = roadParentNodeId;
			DistanceToRoad = distanceToRoad;
		}
	}

	private readonly Dictionary<int, Node> _nodes = new Dictionary<int, Node>();

	public bool IsFilled { get; private set; }

	public void FinishFilling()
	{
		IsFilled = true;
	}

	public void AddNode(int nodeId, int parentNodeId, int roadParentNodeId, float distanceToRoad)
	{
		_nodes[nodeId] = new Node(parentNodeId, roadParentNodeId, distanceToRoad);
	}

	public float GetDistanceToRoad(int nodeId)
	{
		return _nodes[nodeId].DistanceToRoad;
	}

	public int GetParentId(int nodeId)
	{
		return _nodes[nodeId].ParentNodeId;
	}

	public int GetRoadParentNodeId(int nodeId)
	{
		return _nodes[nodeId].RoadParentNodeId;
	}

	public bool HasNode(int nodeId)
	{
		return _nodes.ContainsKey(nodeId);
	}

	public IEnumerable<int> GetAllNodes()
	{
		return _nodes.Keys;
	}

	public void Clear()
	{
		_nodes.Clear();
		IsFilled = false;
	}
}
