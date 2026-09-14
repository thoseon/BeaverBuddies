using System;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectDebuggingPanel : ILoadableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly UILayout _uiLayout;

	private readonly DebugModeManager _debugModeManager;

	private readonly DebugPanelMover _debugPanelMover;

	private readonly ObjectSelector _objectSelector;

	private readonly ObjectViewer _objectViewer;

	private VisualElement _root;

	public ObjectDebuggingPanel(VisualElementLoader visualElementLoader, EventBus eventBus, UILayout uiLayout, DebugModeManager debugModeManager, DebugPanelMover debugPanelMover, ObjectSelector objectSelector, ObjectViewer objectViewer)
	{
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_uiLayout = uiLayout;
		_debugModeManager = debugModeManager;
		_debugPanelMover = debugPanelMover;
		_objectSelector = objectSelector;
		_objectViewer = objectViewer;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/DebuggingPanel/ObjectDebuggingPanel");
		_eventBus.Register(this);
		_uiLayout.AddAbsoluteItem(_root);
		_debugPanelMover.Initialize("ObjectDebuggingPanel", _root, _root.Q<VisualElement>("Wrapper"));
		_objectSelector.Initialize(_root.Q<VisualElement>("ObjectSelector"));
		_objectSelector.SelectedObjectChanged += OnSelectedObjectChanged;
		_objectSelector.ContextChanged += OnContextChanged;
		_objectViewer.Initialize(_root.Q<ScrollView>("ObjectViewer"));
		UpdateEnabledState();
	}

	[OnEvent]
	public void OnDebugModeToggled(DebugModeToggledEvent debugModeToggledEvent)
	{
		UpdateEnabledState();
	}

	public void ResetPanelPosition()
	{
		_debugPanelMover.ResetPanelPosition();
	}

	private void OnSelectedObjectChanged(object sender, object selectedObject)
	{
		_objectViewer.SetObject(selectedObject);
	}

	private void OnContextChanged(object sender, EventArgs e)
	{
		_objectViewer.Clear();
	}

	private void UpdateEnabledState()
	{
		if (_debugModeManager.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			_objectSelector.Enable();
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
			_objectSelector.Disable();
			_objectViewer.Clear();
		}
	}
}
