using Timberborn.Debugging;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Diagnostics;

public class SelectedMeshMetrics : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly MeshMetricsRetriever _meshMetricsRetriever;

	private readonly DebugModeManager _debugModeManager;

	public MeshMetrics MeshMetrics { get; private set; }

	public SelectedMeshMetrics(EventBus eventBus, MeshMetricsRetriever meshMetricsRetriever, DebugModeManager debugModeManager)
	{
		_eventBus = eventBus;
		_meshMetricsRetriever = meshMetricsRetriever;
		_debugModeManager = debugModeManager;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		if (_debugModeManager.Enabled)
		{
			SelectableObject selectableObject = selectableObjectSelectedEvent.SelectableObject;
			MeshMetrics = (selectableObject ? _meshMetricsRetriever.GetMeshMetrics(selectableObject.GameObject) : null);
		}
	}

	[OnEvent]
	public void OnSelectableObjectUnselectedEvent(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		MeshMetrics = null;
	}
}
