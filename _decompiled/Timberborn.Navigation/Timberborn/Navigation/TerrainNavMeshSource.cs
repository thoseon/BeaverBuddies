namespace Timberborn.Navigation;

internal class TerrainNavMeshSource : NavMeshSource
{
	private readonly NodeIdService _nodeIdService;

	private readonly TerrainNavMeshGraph _terrainNavMeshGraph;

	private NavMeshSourceNode[] _nodes;

	public TerrainNavMeshSource(NodeIdService nodeIdService, TerrainNavMeshGraph terrainNavMeshGraph)
		: base(nodeIdService, terrainNavMeshGraph)
	{
	}
}
