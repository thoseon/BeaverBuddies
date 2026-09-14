using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.SingletonSystem;
using Timberborn.Versioning;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.TitleScreenUI;

public class TitleScreenFooter
{
	private readonly EventBus _eventBus;

	private readonly DevModeManager _devModeManager;

	private VisualElement _devModeAlert;

	public VisualElement Root { get; private set; }

	public TitleScreenFooter(EventBus eventBus, DevModeManager devModeManager)
	{
		_eventBus = eventBus;
		_devModeManager = devModeManager;
	}

	public void Initialize(VisualElement parent)
	{
		Root = parent.Q<VisualElement>("Footer");
		Root.Q<Label>("GameVersion").text = GameVersions.CurrentVersion.Formatted;
		_devModeAlert = Root.Q<Label>("DevModeAlert");
		UpdateDevModeAlert(newState: false);
		_eventBus.Register(this);
		Root.ToggleDisplayStyle(visible: false);
	}

	public void Show()
	{
		Root.ToggleDisplayStyle(visible: true);
	}

	[OnEvent]
	public void OnDevModeToggled(DevModeToggledEvent devModeToggledEvent)
	{
		UpdateDevModeAlert(_devModeManager.Enabled);
	}

	private void UpdateDevModeAlert(bool newState)
	{
		_devModeAlert.ToggleDisplayStyle(!Application.isEditor & newState);
	}
}
