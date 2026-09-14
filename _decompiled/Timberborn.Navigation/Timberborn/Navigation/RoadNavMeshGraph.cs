using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class RoadNavMeshGraph : INavMeshGraph, ILoadableSingleton
{
	private static readonly List<NavMeshNode> EmptyNeighbors = new List<NavMeshNode>();

	private readonly NodeIdService _nodeIdService;

	private List<NavMeshNode>[] _neighbors;

	public RoadNavMeshGraph(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public void Load()
	{
		_neighbors = new List<NavMeshNode>[_nodeIdService.NumberOfNodes];
		for (int i = 0; i < _neighbors.Length; i++)
		{
			_neighbors[i] = EmptyNeighbors;
		}
	}

	public void ConnectNodes(int aNodeId, int bNodeId, int groupId, float cost)
	{
		VerifyBeforeChange(aNodeId, bNodeId);
		RemoveOneWayConnections(aNodeId, bNodeId);
		AddOneWayConnection(aNodeId, bNodeId, groupId, cost);
		AddOneWayConnection(bNodeId, aNodeId, groupId, cost);
		VerifyAfterChange(aNodeId, bNodeId);
	}

	public void DisconnectNodes(int aNodeId, int bNodeId)
	{
		RemoveOneWayConnections(aNodeId, bNodeId);
		VerifyAfterChange(aNodeId, bNodeId);
	}

	public ReadOnlyList<NavMeshNode> GetNeighbors(int nodeId)
	{
		return _neighbors[nodeId].AsReadOnlyList();
	}

	public bool IsOnNavMesh(int nodeId)
	{
		return !_neighbors[nodeId].IsEmpty();
	}

	public bool AreConnected(int nodeIdA, int nodeIdB)
	{
		List<NavMeshNode> list = _neighbors[nodeIdA];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Id == nodeIdB)
			{
				return true;
			}
		}
		return false;
	}

	private void RemoveOneWayConnections(int aNodeId, int bNodeId)
	{
		RemoveOneWayConnection(aNodeId, bNodeId);
		RemoveOneWayConnection(bNodeId, aNodeId);
	}

	private void AddOneWayConnection(int aNodeId, int bNodeId, int groupId, float cost)
	{
		_neighbors[aNodeId].Add(new NavMeshNode(bNodeId, groupId, cost));
	}

	private void RemoveOneWayConnection(int aNodeId, int bNodeId)
	{
		List<NavMeshNode> list = _neighbors[aNodeId];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].Id == bNodeId)
			{
				list.RemoveAt(num);
			}
		}
	}

	private void VerifyBeforeChange(int aNodeId, int bNodeId)
	{
		VerifyBeforeChange(aNodeId);
		VerifyBeforeChange(bNodeId);
	}

	private void VerifyBeforeChange(int nodeId)
	{
		if (_neighbors[nodeId] == EmptyNeighbors)
		{
			_neighbors[nodeId] = new List<NavMeshNode>();
		}
	}

	private void VerifyAfterChange(int aNodeId, int bNodeId)
	{
		VerifyAfterChange(aNodeId);
		VerifyAfterChange(bNodeId);
	}

	private void VerifyAfterChange(int nodeId)
	{
		if (_neighbors[nodeId].Count == 0)
		{
			_neighbors[nodeId] = EmptyNeighbors;
		}
	}
}
