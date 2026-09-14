using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class RestrictedNodeMap : ILoadableSingleton
{
	private readonly NodeIdService _nodeIdService;

	private bool[] _nodes;

	public RestrictedNodeMap(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public void Load()
	{
		_nodes = new bool[_nodeIdService.NumberOfNodes];
	}

	public bool IsNodeRestricted(int nodeId)
	{
		return _nodes[nodeId];
	}

	public void RestrictNode(int nodeId)
	{
		_nodes[nodeId] = true;
	}

	public void UnrestrictNode(int nodeId)
	{
		_nodes[nodeId] = false;
	}
}
