using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.ToolButtonSystem;

public class ToolGroupButtonFactory
{
	private static readonly string GreenClass = "bottom-bar-button--green";

	private static readonly string BlueClass = "bottom-bar-button--blue";

	private readonly EventBus _eventBus;

	private readonly ToolGroupService _toolGroupService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ToolButtonService _toolButtonService;

	private readonly ILoc _loc;

	public ToolGroupButtonFactory(EventBus eventBus, ToolGroupService toolGroupService, VisualElementLoader visualElementLoader, ToolButtonService toolButtonService, ILoc loc)
	{
		_eventBus = eventBus;
		_toolGroupService = toolGroupService;
		_visualElementLoader = visualElementLoader;
		_toolButtonService = toolButtonService;
		_loc = loc;
	}

	public ToolGroupButton CreateGreen(ToolGroupSpec toolGroup)
	{
		return Create(toolGroup, GreenClass);
	}

	public ToolGroupButton CreateBlue(ToolGroupSpec toolGroup)
	{
		return Create(toolGroup, BlueClass);
	}

	private ToolGroupButton Create(ToolGroupSpec toolGroup, string className)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/BottomBar/ToolGroupButton");
		visualElement.Q<VisualElement>("ToolGroupButtonWrapper").AddToClassList(className);
		InitializeElement(visualElement, toolGroup);
		ToolGroupButton toolGroupButton = new ToolGroupButton(_loc, _toolGroupService, toolGroup, visualElement, visualElement.Q<VisualElement>("ToolButtons"), visualElement.Q<VisualElement>("ToolGroupButtonWrapper"));
		_eventBus.Register(toolGroupButton);
		_toolButtonService.Add(toolGroupButton);
		return toolGroupButton;
	}

	private void InitializeElement(VisualElement root, ToolGroupSpec toolGroup)
	{
		Button button = root.Q<Button>("ToolGroupButton");
		Label tooltip = root.Q<Label>("Tooltip");
		button.RegisterCallback<MouseEnterEvent>(delegate
		{
			tooltip.parent.ToggleDisplayStyle(visible: true);
		});
		button.RegisterCallback<MouseLeaveEvent>(delegate
		{
			tooltip.parent.ToggleDisplayStyle(visible: false);
		});
		button.RegisterCallback<ClickEvent>(delegate
		{
			OnButtonClick(tooltip, toolGroup);
		});
		button.style.backgroundImage = new StyleBackground(toolGroup.Icon.Asset);
		tooltip.parent.ToggleDisplayStyle(visible: false);
		root.Q<VisualElement>("ToolButtons").ToggleDisplayStyle(visible: false);
	}

	private void OnButtonClick(Label tooltip, ToolGroupSpec toolGroup)
	{
		if (_toolGroupService.ActiveToolGroup == toolGroup)
		{
			_toolGroupService.ExitToolGroup();
			return;
		}
		_toolGroupService.EnterToolGroup(toolGroup);
		tooltip.parent.ToggleDisplayStyle(visible: false);
	}
}
