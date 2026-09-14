namespace Timberborn.Navigation;

internal class InstantTerrainNavMeshSource : TerrainNavMeshSource
{
	public InstantTerrainNavMeshSource(NodeIdService nodeIdService, InstantTerrainNavMeshGraph instantTerrainNavMeshGraph)
		: base(nodeIdService, instantTerrainNavMeshGraph)
	{
	}
}
