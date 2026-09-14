using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

public class DebuggingPanel : IUpdatableSingleton, ILoadableSingleton
{
	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly DebugModeManager _debugModeManager;

	private readonly ISettings _settings;

	private readonly DebugPanelMover _debugPanelMover;

	private VisualElement _root;

	private VisualElement _content;

	private readonly List<DebuggingPanelItem> _debuggingPanelItems = new List<DebuggingPanelItem>();

	public DebuggingPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, EventBus eventBus, DebugModeManager debugModeManager, ISettings settings, DebugPanelMover debugPanelMover)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_debugModeManager = debugModeManager;
		_settings = settings;
		_debugPanelMover = debugPanelMover;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/DebuggingPanel/DebuggingPanel");
		_root.ToggleDisplayStyle(_debugModeManager.Enabled);
		_eventBus.Register(this);
		_uiLayout.AddAbsoluteItem(_root);
		_content = _root.Q<VisualElement>("Content");
		_debugPanelMover.Initialize("DebuggingPanel", _root, _content);
	}

	public void UpdateSingleton()
	{
		if (!_debugModeManager.Enabled)
		{
			return;
		}
		foreach (DebuggingPanelItem debuggingPanelItem in _debuggingPanelItems)
		{
			debuggingPanelItem.UpdateText();
		}
	}

	public void AddDebuggingPanel(IDebuggingPanel debuggingPanel, string title)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/DebuggingPanel/DebuggingPanelItem");
		_content.Add(visualElement);
		DebuggingPanelItem debuggingPanelItem = new DebuggingPanelItem(_settings, debuggingPanel, visualElement, title);
		debuggingPanelItem.Initialize();
		_debuggingPanelItems.Add(debuggingPanelItem);
	}

	[OnEvent]
	public void OnDebugModeToggled(DebugModeToggledEvent debugModeToggledEvent)
	{
		_root.ToggleDisplayStyle(debugModeToggledEvent.Enabled);
	}

	public void ResetPanelPosition()
	{
		_debugPanelMover.ResetPanelPosition();
	}
}
