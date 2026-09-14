namespace Timberborn.Navigation;

internal class PreviewTerrainNavMeshGraph : TerrainNavMeshGraph
{
	public PreviewTerrainNavMeshGraph(NodeIdService nodeIdService, NavMeshGroupService navMeshGroupService)
		: base(nodeIdService, navMeshGroupService)
	{
	}
}
