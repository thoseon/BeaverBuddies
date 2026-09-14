using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class TerrainNavMeshGraph : INavMeshGraph, ILoadableSingleton
{
	private static readonly float DefaultConnectionCost = 1f;

	private static readonly List<NavMeshNode> EmptyAllNeighbors = new List<NavMeshNode>();

	private static readonly List<int> EmptyCheapNeighbors = new List<int>();

	private readonly NodeIdService _nodeIdService;

	private readonly NavMeshGroupService _navMeshGroupService;

	private List<NavMeshNode>[] _allNeighbors;

	private List<int>[] _cheapNeighbors;

	private int _defaultGroupId;

	public TerrainNavMeshGraph(NodeIdService nodeIdService, NavMeshGroupService navMeshGroupService)
	{
		_nodeIdService = nodeIdService;
		_navMeshGroupService = navMeshGroupService;
	}

	public void Load()
	{
		_allNeighbors = new List<NavMeshNode>[_nodeIdService.NumberOfNodes];
		_cheapNeighbors = new List<int>[_nodeIdService.NumberOfNodes];
		for (int i = 0; i < _allNeighbors.Length; i++)
		{
			_allNeighbors[i] = EmptyAllNeighbors;
			_cheapNeighbors[i] = EmptyCheapNeighbors;
		}
		_defaultGroupId = _navMeshGroupService.GetDefaultGroupId();
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
		return _allNeighbors[nodeId].AsReadOnlyList();
	}

	public ReadOnlyList<int> GetCheapNeighbors(int nodeId)
	{
		return _cheapNeighbors[nodeId].AsReadOnlyList();
	}

	public bool IsOnNavMesh(int nodeId)
	{
		return !_allNeighbors[nodeId].IsEmpty();
	}

	public bool AreConnected(int nodeIdA, int nodeIdB)
	{
		List<NavMeshNode> list = _allNeighbors[nodeIdA];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Id == nodeIdB)
			{
				return true;
			}
		}
		return false;
	}

	public float GetConnectionCost(int nodeIdA, int nodeIdB)
	{
		List<NavMeshNode> list = _allNeighbors[nodeIdA];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Id == nodeIdB)
			{
				return list[i].Cost;
			}
		}
		return DefaultConnectionCost;
	}

	public int GetGroupId(int nodeIdA, int nodeIdB)
	{
		List<NavMeshNode> list = _allNeighbors[nodeIdA];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Id == nodeIdB)
			{
				return list[i].GroupId;
			}
		}
		return _defaultGroupId;
	}

	public bool IsConnectedToDefaultGroup(int nodeId)
	{
		List<NavMeshNode> list = _allNeighbors[nodeId];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].GroupId == _defaultGroupId)
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
		_allNeighbors[aNodeId].Add(new NavMeshNode(bNodeId, groupId, cost));
		if (cost <= 1f)
		{
			_cheapNeighbors[aNodeId].Add(bNodeId);
		}
	}

	private void RemoveOneWayConnection(int aNodeId, int bNodeId)
	{
		List<NavMeshNode> list = _allNeighbors[aNodeId];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].Id == bNodeId)
			{
				list.RemoveAt(num);
			}
		}
		_cheapNeighbors[aNodeId].Remove(bNodeId);
	}

	private void VerifyBeforeChange(int aNodeId, int bNodeId)
	{
		VerifyBeforeChange(aNodeId);
		VerifyBeforeChange(bNodeId);
	}

	private void VerifyBeforeChange(int nodeId)
	{
		if (_allNeighbors[nodeId] == EmptyAllNeighbors)
		{
			_allNeighbors[nodeId] = new List<NavMeshNode>();
		}
		if (_cheapNeighbors[nodeId] == EmptyCheapNeighbors)
		{
			_cheapNeighbors[nodeId] = new List<int>();
		}
	}

	private void VerifyAfterChange(int aNodeId, int bNodeId)
	{
		VerifyAfterChange(aNodeId);
		VerifyAfterChange(bNodeId);
	}

	private void VerifyAfterChange(int nodeId)
	{
		if (_allNeighbors[nodeId].Count == 0)
		{
			_allNeighbors[nodeId] = EmptyAllNeighbors;
		}
		if (_cheapNeighbors[nodeId].Count == 0)
		{
			_cheapNeighbors[nodeId] = EmptyCheapNeighbors;
		}
	}
}
