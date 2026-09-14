using Timberborn.Common;

namespace Timberborn.Navigation;

internal class TerrainFlowFieldCache : IPrioritizedSingletonNavMeshListener
{
	private readonly FlowFieldCache _flowFields = new FlowFieldCache();

	private readonly PathFlowField _defaultFlowField = new PathFlowField();

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		ReadOnlyList<int> terrainNodeIds = navMeshUpdate.TerrainNodeIds;
		_defaultFlowField.OnNodesChanged(terrainNodeIds);
		_flowFields.OnNodesChanged(terrainNodeIds);
	}

	public PathFlowField GetDefaultFlowField()
	{
		return _defaultFlowField;
	}

	public AccessFlowField GetFlowFieldAtNode(int nodeId)
	{
		return _flowFields.GetFlowFieldAtNode(nodeId);
	}

	public bool TryGetFlowFieldAtNode(int nodeId, out AccessFlowField flowField)
	{
		return _flowFields.TryGetFlowFieldAtNode(nodeId, out flowField);
	}

	public void StartCachingAtNode(int nodeId)
	{
		_flowFields.StartCachingAtNode(nodeId);
	}

	public void StopCachingAtNode(int nodeId)
	{
		_flowFields.StopCachingAtNode(nodeId);
	}
}
