using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class PathFlowField : IFlowField
{
	private readonly struct VisitedNode
	{
		public int ParentNodeId { get; }

		public float Distance { get; }

		public VisitedNode(int parentNodeId, float distance)
		{
			ParentNodeId = parentNodeId;
			Distance = distance;
		}
	}

	private readonly Dictionary<int, VisitedNode> _nodes = new Dictionary<int, VisitedNode>();

	private int _startNodeId = -1;

	private bool _refreshed;

	private bool _fullyFilled;

	public bool CheckedPath(int startNodeId, int destinationNodeId)
	{
		if (!FoundPath(startNodeId, destinationNodeId))
		{
			if (startNodeId == _startNodeId && _fullyFilled)
			{
				return _refreshed;
			}
			return false;
		}
		return true;
	}

	public bool FoundPath(int startNodeId, int destinationNodeId)
	{
		if (startNodeId == _startNodeId)
		{
			return HasNode(destinationNodeId);
		}
		return false;
	}

	public void MarkAsFullyFilled()
	{
		_fullyFilled = true;
	}

	public void MarkAsPartiallyFilled()
	{
		_fullyFilled = false;
	}

	public void AddNode(int nodeId, int parentNodeId, float distance)
	{
		_nodes[nodeId] = new VisitedNode(parentNodeId, distance);
	}

	public bool HasNode(int nodeId)
	{
		return _nodes.ContainsKey(nodeId);
	}

	public int GetParentId(int nodeId)
	{
		return _nodes[nodeId].ParentNodeId;
	}

	public float GetDistance(int nodeId)
	{
		return _nodes[nodeId].Distance;
	}

	public void OnNodesChanged(ReadOnlyList<int> nodeIds)
	{
		for (int i = 0; i < nodeIds.Count; i++)
		{
			if (HasNode(nodeIds[i]))
			{
				Clear(_startNodeId, refreshed: false);
				break;
			}
		}
	}

	public void Clear(int startNodeId)
	{
		Clear(startNodeId, refreshed: true);
	}

	private void Clear(int startNodeId, bool refreshed)
	{
		_startNodeId = startNodeId;
		_refreshed = refreshed;
		_nodes.Clear();
		_fullyFilled = false;
	}
}
