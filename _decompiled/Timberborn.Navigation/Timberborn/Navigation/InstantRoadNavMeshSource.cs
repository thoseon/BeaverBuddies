namespace Timberborn.Navigation;

internal class InstantRoadNavMeshSource : RoadNavMeshSource
{
	public InstantRoadNavMeshSource(NodeIdService nodeIdService, InstantRoadNavMeshGraph instantRoadNavMeshGraph)
		: base(nodeIdService, instantRoadNavMeshGraph)
	{
	}
}
