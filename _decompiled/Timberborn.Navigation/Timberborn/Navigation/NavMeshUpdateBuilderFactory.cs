namespace Timberborn.Navigation;

internal class NavMeshUpdateBuilderFactory
{
	private readonly NodeIdService _nodeIdService;

	public NavMeshUpdateBuilderFactory(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public NavMeshUpdate.Builder Create()
	{
		return new NavMeshUpdate.Builder(_nodeIdService);
	}
}
