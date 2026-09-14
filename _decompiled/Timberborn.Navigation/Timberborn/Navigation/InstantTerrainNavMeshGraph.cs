namespace Timberborn.Navigation;

internal class InstantTerrainNavMeshGraph : TerrainNavMeshGraph
{
	public InstantTerrainNavMeshGraph(NodeIdService nodeIdService, NavMeshGroupService navMeshGroupService)
		: base(nodeIdService, navMeshGroupService)
	{
	}
}
