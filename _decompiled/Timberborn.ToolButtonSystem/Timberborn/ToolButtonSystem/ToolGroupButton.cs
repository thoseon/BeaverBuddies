using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.ToolButtonSystem;

public class ToolGroupButton : IToolbarButton
{
	private static readonly string ActiveClassName = "button--active";

	private readonly ILoc _loc;

	private readonly ToolGroupService _toolGroupService;

	private readonly VisualElement _toolGroupButtonWrapper;

	private readonly ToolGroupSpec _toolGroup;

	private readonly List<ToolButton> _toolButtons = new List<ToolButton>();

	public VisualElement Root { get; }

	public VisualElement ToolButtonsElement { get; }

	public ReadOnlyList<ToolButton> ToolButtons => _toolButtons.AsReadOnlyList();

	public bool IsVisible => _toolButtons.Any((ToolButton button) => button.ToolEnabled);

	public bool IsActive => _toolGroupService.ActiveToolGroup == _toolGroup;

	public ToolGroupButton(ILoc loc, ToolGroupService toolGroupService, ToolGroupSpec toolGroup, VisualElement root, VisualElement toolButtons, VisualElement buttonWrapper)
	{
		_loc = loc;
		_toolGroupService = toolGroupService;
		_toolGroup = toolGroup;
		Root = root;
		ToolButtonsElement = toolButtons;
		_toolGroupButtonWrapper = buttonWrapper;
	}

	public void PostLoad()
	{
		Root.Q<Label>("Tooltip").text = _loc.T(_toolGroup.DisplayNameLocKey);
		ToolButtonsElement.Q<VisualElement>("EndSpacer").BringToFront();
		_toolGroupButtonWrapper.ToggleDisplayStyle(IsVisible);
	}

	public void AddTool(ToolButton button)
	{
		_toolButtons.Add(button);
	}

	[OnEvent]
	public void OnToolGroupEntered(ToolGroupEnteredEvent toolGroupOpenedEvent)
	{
		if (toolGroupOpenedEvent.ToolGroup == _toolGroup)
		{
			ToolButtonsElement.ToggleDisplayStyle(visible: true);
			_toolGroupButtonWrapper.AddToClassList(ActiveClassName);
		}
	}

	[OnEvent]
	public void OnToolGroupExited(ToolGroupExitedEvent toolGroupExitedEvent)
	{
		if (toolGroupExitedEvent.ToolGroup == _toolGroup)
		{
			ToolButtonsElement.ToggleDisplayStyle(visible: false);
			_toolGroupButtonWrapper.RemoveFromClassList(ActiveClassName);
		}
	}

	[OnEvent]
	public void OnDevModeToggledEvent(DevModeToggledEvent devModeToggledEvent)
	{
		_toolGroupButtonWrapper.ToggleDisplayStyle(IsVisible);
	}

	public bool HasToolButton(ToolButton toolButton)
	{
		return _toolButtons.Contains(toolButton);
	}

	public void Select()
	{
		_toolGroupService.EnterToolGroup(_toolGroup);
	}
}
