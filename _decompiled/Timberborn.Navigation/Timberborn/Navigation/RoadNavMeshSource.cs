namespace Timberborn.Navigation;

internal class RoadNavMeshSource : NavMeshSource
{
	private readonly NodeIdService _nodeIdService;

	private readonly RoadNavMeshGraph _roadNavMeshGraph;

	private NavMeshSourceNode[] _nodes;

	public RoadNavMeshSource(NodeIdService nodeIdService, RoadNavMeshGraph roadNavMeshGraph)
		: base(nodeIdService, roadNavMeshGraph)
	{
	}
}
