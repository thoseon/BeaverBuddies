using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.ToolSystemUI;

internal class DescriptionPanelController : IToolFragment, IUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private readonly DescriptionPanels _descriptionPanels;

	private ITool _permanentTool;

	private readonly VisualElement _root = new VisualElement();

	private DescriptionPanel _shownPanel;

	public DescriptionPanelController(EventBus eventBus, DescriptionPanels descriptionPanels)
	{
		_eventBus = eventBus;
		_descriptionPanels = descriptionPanels;
	}

	public VisualElement InitializeFragment()
	{
		_eventBus.Register(this);
		Hide();
		return _root;
	}

	public void UpdateSingleton()
	{
		_shownPanel?.Update();
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_permanentTool = toolEnteredEvent.Tool;
		SetDescription(_permanentTool);
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		Hide();
	}

	[OnEvent]
	public void OnToolUnlocked(ToolUnlockedEvent toolUnlockedEvent)
	{
		if (toolUnlockedEvent.Tool == _permanentTool)
		{
			SetDescription(_permanentTool);
		}
	}

	[OnEvent]
	public void OnTemporaryToolEntered(TemporaryToolEnteredEvent temporaryToolEnteredEvent)
	{
		SetTemporaryTool(temporaryToolEnteredEvent.Tool);
	}

	[OnEvent]
	public void OnTemporaryToolExited(TemporaryToolExitedEvent temporaryToolExitedEvent)
	{
		ClearTemporaryTool();
	}

	private void SetTemporaryTool(ITool tool)
	{
		SetDescription(tool);
	}

	private void ClearTemporaryTool()
	{
		SetDescription(_permanentTool);
	}

	private void SetDescription(ITool tool)
	{
		Hide();
		if (tool is IToolDescriptor toolDescriptor)
		{
			DescriptionPanel descriptionPanel = _descriptionPanels.GetDescriptionPanel(toolDescriptor);
			Show(descriptionPanel);
		}
	}

	private void Show(DescriptionPanel panel)
	{
		_shownPanel = panel;
		panel.Update();
		_root.Add(panel.Root);
		_root.ToggleDisplayStyle(visible: true);
	}

	private void Hide()
	{
		_shownPanel = null;
		_root.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}
}
