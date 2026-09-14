using Timberborn.Common;

namespace Timberborn.Navigation;

internal class RoadFlowFieldCache : IPrioritizedSingletonNavMeshListener, IPrioritizedSingletonInstantNavMeshListener
{
	private readonly FlowFieldCache _flowFields = new FlowFieldCache();

	private readonly PathFlowField _defaultFlowField = new PathFlowField();

	private readonly AccessFlowField _instantFlowField = new AccessFlowField();

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		ReadOnlyList<int> roadNodeIds = navMeshUpdate.RoadNodeIds;
		_defaultFlowField.OnNodesChanged(roadNodeIds);
		_flowFields.OnNodesChanged(roadNodeIds);
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_instantFlowField.OnNodesChanged(navMeshUpdate.RoadNodeIds);
	}

	public PathFlowField GetDefaultFlowField()
	{
		return _defaultFlowField;
	}

	public AccessFlowField GetInstantFlowField(int nodeId)
	{
		if (!_instantFlowField.HasNode(nodeId))
		{
			_instantFlowField.Clear();
		}
		return _instantFlowField;
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
