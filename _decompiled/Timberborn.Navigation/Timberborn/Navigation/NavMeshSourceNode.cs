using System;
using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Navigation;

internal class NavMeshSourceNode
{
	private static readonly List<NavMeshNode> EmptyEdges = new List<NavMeshNode>();

	private static readonly List<int> EmptyBlockages = new List<int>();

	private static readonly HashSet<int> DistinctBlockages = new HashSet<int>();

	private List<NavMeshNode> _edges = EmptyEdges;

	private List<int> _blockages = EmptyBlockages;

	public bool IsEmpty
	{
		get
		{
			if (_edges == EmptyEdges)
			{
				return _blockages == EmptyBlockages;
			}
			return false;
		}
	}

	public void AddEdge(int nodeId, float cost, int groupId)
	{
		if (_edges == EmptyEdges)
		{
			_edges = new List<NavMeshNode>();
		}
		_edges.Add(new NavMeshNode(nodeId, groupId, cost));
	}

	public void RemoveEdge(int nodeId, float cost, int groupId)
	{
		_edges.Remove(new NavMeshNode(nodeId, groupId, cost));
		if (_edges.Count == 0)
		{
			_edges = EmptyEdges;
		}
	}

	public void BlockEdge(int nodeId, int groupId)
	{
		if (_blockages == EmptyBlockages)
		{
			_blockages = new List<int>();
		}
		_blockages.Add(GetBlockageKey(nodeId, groupId));
	}

	public void UnblockEdge(int nodeId, int groupId)
	{
		if (!_blockages.Remove(GetBlockageKey(nodeId, groupId)))
		{
			throw new InvalidOperationException($"Can't unblock edge to {nodeId}, it wasn't blocked");
		}
		if (_blockages.Count == 0)
		{
			_blockages = EmptyBlockages;
		}
	}

	public bool IsConnectedTo(int nodeId, out int groupId, out float cost)
	{
		cost = float.MaxValue;
		groupId = 0;
		bool result = false;
		DistinctBlockages.Clear();
		DistinctBlockages.AddRange(_blockages);
		for (int i = 0; i < _edges.Count; i++)
		{
			NavMeshNode navMeshNode = _edges[i];
			int blockageKey = GetBlockageKey(navMeshNode.Id, navMeshNode.GroupId);
			if (navMeshNode.Id == nodeId && navMeshNode.Cost < cost && !DistinctBlockages.Remove(blockageKey))
			{
				result = true;
				cost = navMeshNode.Cost;
				groupId = navMeshNode.GroupId;
			}
		}
		return result;
	}

	private static int GetBlockageKey(int nodeId, int groupId)
	{
		return (nodeId * 397) ^ groupId;
	}
}
