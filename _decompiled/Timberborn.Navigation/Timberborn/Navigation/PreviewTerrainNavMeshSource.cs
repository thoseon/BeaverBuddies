namespace Timberborn.Navigation;

internal class PreviewTerrainNavMeshSource : TerrainNavMeshSource
{
	public PreviewTerrainNavMeshSource(NodeIdService nodeIdService, PreviewTerrainNavMeshGraph previewTerrainNavMeshGraph)
		: base(nodeIdService, previewTerrainNavMeshGraph)
	{
	}
}
