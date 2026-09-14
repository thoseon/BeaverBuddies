namespace Timberborn.Navigation;

internal class NavMeshChangeFactory
{
	private readonly NodeIdService _nodeIdService;

	public NavMeshChangeFactory(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public NavMeshChange Create(in NavMeshChangeSpecification navMeshChangeSpecification)
	{
		NavMeshEdge edge = navMeshChangeSpecification.NavMeshEdge;
		if (EdgeIsInBounds(in edge))
		{
			NavMeshChangeType navMeshChangeType = navMeshChangeSpecification.NavMeshChangeType;
			int startNodeId = _nodeIdService.GridToId(edge.Start);
			int endNodeId = _nodeIdService.GridToId(edge.End);
			int groupId = navMeshChangeSpecification.NavMeshEdge.GroupId;
			float cost = EdgeCost(navMeshChangeType, in edge);
			return new NavMeshChange(navMeshChangeType, startNodeId, endNodeId, groupId, cost);
		}
		return default(NavMeshChange);
	}

	private bool EdgeIsInBounds(in NavMeshEdge edge)
	{
		if (_nodeIdService.Contains(edge.Start))
		{
			return _nodeIdService.Contains(edge.End);
		}
		return false;
	}

	private static float EdgeCost(NavMeshChangeType navMeshChangeType, in NavMeshEdge navMeshEdge)
	{
		if ((navMeshChangeType != NavMeshChangeType.BlockEdge && navMeshChangeType != NavMeshChangeType.UnblockEdge) || 1 == 0)
		{
			return navMeshEdge.Cost;
		}
		return 0f;
	}
}
