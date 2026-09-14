using System;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal abstract class NavMeshSource : ILoadableSingleton
{
	private readonly NodeIdService _nodeIdService;

	private readonly INavMeshGraph _navMeshGraph;

	private NavMeshSourceNode[] _nodes;

	protected NavMeshSource(NodeIdService nodeIdService, INavMeshGraph navMeshGraph)
	{
		_nodeIdService = nodeIdService;
		_navMeshGraph = navMeshGraph;
	}

	public void Load()
	{
		_nodes = new NavMeshSourceNode[_nodeIdService.NumberOfNodes];
	}

	public void AddEdge(int startNodeId, int endNodeId, int groupId, float cost)
	{
		VerifyBeforeChange(startNodeId, endNodeId);
		GetNode(startNodeId).AddEdge(endNodeId, cost, groupId);
		UpdateConnectionBetweenNodes(startNodeId, endNodeId);
	}

	public void RemoveEdge(int startNodeId, int endNodeId, int group, float cost)
	{
		VerifyBeforeChange(startNodeId, endNodeId);
		GetNode(startNodeId).RemoveEdge(endNodeId, cost, group);
		UpdateConnectionBetweenNodes(startNodeId, endNodeId);
		VerifyAfterChange(startNodeId, endNodeId);
	}

	public void BlockEdge(int startNodeId, int endNodeId, int groupId)
	{
		VerifyBeforeChange(startNodeId, endNodeId);
		GetNode(startNodeId).BlockEdge(endNodeId, groupId);
		UpdateConnectionBetweenNodes(startNodeId, endNodeId);
	}

	public void UnblockEdge(int startNodeId, int endNodeId, int groupId)
	{
		VerifyBeforeChange(startNodeId, endNodeId);
		GetNode(startNodeId).UnblockEdge(endNodeId, groupId);
		UpdateConnectionBetweenNodes(startNodeId, endNodeId);
		VerifyAfterChange(startNodeId, endNodeId);
	}

	private void VerifyBeforeChange(int startNodeId, int endNodeId)
	{
		VerifyBeforeChange(startNodeId);
		VerifyBeforeChange(endNodeId);
	}

	private void VerifyBeforeChange(int nodeId)
	{
		NavMeshSourceNode[] nodes = _nodes;
		if (nodes[nodeId] == null)
		{
			nodes[nodeId] = new NavMeshSourceNode();
		}
	}

	private void VerifyAfterChange(int startNodeId, int endNodeId)
	{
		VerifyAfterChange(startNodeId);
		VerifyAfterChange(endNodeId);
	}

	private void VerifyAfterChange(int nodeId)
	{
		if (_nodes[nodeId].IsEmpty)
		{
			_nodes[nodeId] = null;
		}
	}

	private NavMeshSourceNode GetNode(int nodeId)
	{
		return _nodes[nodeId];
	}

	private void UpdateConnectionBetweenNodes(int aNodeId, int bNodeId)
	{
		NavMeshSourceNode node = GetNode(aNodeId);
		NavMeshSourceNode node2 = GetNode(bNodeId);
		if (node.IsConnectedTo(bNodeId, out var groupId, out var cost) && node2.IsConnectedTo(aNodeId, out var groupId2, out var cost2) && groupId == groupId2)
		{
			float cost3 = Math.Max(cost, cost2);
			_navMeshGraph.ConnectNodes(aNodeId, bNodeId, groupId, cost3);
		}
		else
		{
			_navMeshGraph.DisconnectNodes(aNodeId, bNodeId);
		}
	}
}
