namespace Timberborn.Navigation;

internal class PreviewRoadNavMeshSource : RoadNavMeshSource
{
	public PreviewRoadNavMeshSource(NodeIdService nodeIdService, PreviewRoadNavMeshGraph previewRoadNavMeshGraph)
		: base(nodeIdService, previewRoadNavMeshGraph)
	{
	}
}
