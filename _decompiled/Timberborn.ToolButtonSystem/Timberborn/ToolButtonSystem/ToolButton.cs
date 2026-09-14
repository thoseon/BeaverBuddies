using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.ToolButtonSystem;

public class ToolButton : IToolbarButton
{
	private static readonly string ActiveClassName = "button--active";

	private static readonly string LockedClassName = "button--locked";

	private readonly ToolService _toolService;

	private readonly DevModeManager _devModeManager;

	private readonly ToolGroupService _toolGroupService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly ImmutableArray<IToolDisabler> _toolDisablers;

	private readonly Button _button;

	private Label _tooltip;

	private string _tooltipText;

	private bool _containedMouse;

	public ITool Tool { get; }

	public VisualElement Root { get; }

	public bool ToolEnabled
	{
		get
		{
			if (!_devModeManager.Enabled && !_mapEditorMode.IsMapEditor)
			{
				if (!DevModeTool)
				{
					return _toolDisablers.FastAll((IToolDisabler disabler) => disabler.IsEnabled(Tool));
				}
				return false;
			}
			return true;
		}
	}

	public bool IsVisible
	{
		get
		{
			if (ToolEnabled)
			{
				if (_toolGroupService.IsAssignedToAnyGroup(Tool))
				{
					return _toolGroupService.IsAssignedToGroup(Tool, _toolGroupService.ActiveToolGroup);
				}
				return true;
			}
			return false;
		}
	}

	public bool IsActive => _toolService.ActiveTool == Tool;

	private bool DevModeTool
	{
		get
		{
			if (Tool is IDevModeTool devModeTool)
			{
				return devModeTool.IsDevMode;
			}
			return false;
		}
	}

	public ToolButton(ToolService toolService, DevModeManager devModeManager, ToolGroupService toolGroupService, MapEditorMode mapEditorMode, ImmutableArray<IToolDisabler> toolDisablers, ITool tool, VisualElement root, Button button)
	{
		_toolService = toolService;
		_devModeManager = devModeManager;
		_toolGroupService = toolGroupService;
		_mapEditorMode = mapEditorMode;
		_toolDisablers = toolDisablers;
		Tool = tool;
		Root = root;
		_button = button;
	}

	public void PostLoad()
	{
		_button.RegisterCallback<ClickEvent>(delegate
		{
			_toolService.SwitchTool(Tool);
		});
		_tooltip = Root.Q<Label>("Tooltip");
		Root.RegisterCallback<MouseEnterEvent>(delegate
		{
			_toolService.SetTemporaryTool(Tool);
		});
		Root.RegisterCallback<MouseLeaveEvent>(delegate
		{
			_toolService.ClearTemporaryTool();
		});
		UpdateToolVisibility();
	}

	public void InitializeTooltip(string tooltipText)
	{
		_tooltipText = tooltipText;
		Root.RegisterCallback<MouseOverEvent>(ShowTooltip);
		Root.RegisterCallback<MouseOutEvent>(HideTooltip);
	}

	[OnEvent]
	public void OnDevModeToggledEvent(DevModeToggledEvent devModeToggledEvent)
	{
		UpdateToolVisibility();
	}

	[OnEvent]
	public void OnToolGroupEnteredEvent(ToolGroupEnteredEvent toolGroupEnteredEvent)
	{
		if (_toolGroupService.IsAssignedToGroup(Tool, toolGroupEnteredEvent.ToolGroup))
		{
			UpdateToolVisibility();
		}
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (toolEnteredEvent.Tool == Tool)
		{
			Root.AddToClassList(ActiveClassName);
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		if (toolExitedEvent.Tool == Tool)
		{
			Root.RemoveFromClassList(ActiveClassName);
		}
	}

	[OnEvent]
	public void OnToolLocked(ToolLockedEvent toolLockedEvent)
	{
		if (toolLockedEvent.Tool == Tool)
		{
			_button.AddToClassList(LockedClassName);
		}
	}

	[OnEvent]
	public void OnToolUnlocked(ToolUnlockedEvent toolUnlockedEvent)
	{
		if (toolUnlockedEvent.Tool == Tool)
		{
			_button.RemoveFromClassList(LockedClassName);
		}
	}

	public void Select()
	{
		_toolService.SwitchTool(Tool);
	}

	private void ShowTooltip(MouseOverEvent mouseOverEvent)
	{
		if (_tooltip != null)
		{
			_tooltip.text = _tooltipText;
			_tooltip.ToggleDisplayStyle(visible: true);
		}
	}

	private void HideTooltip(MouseOutEvent mouseOutEvent)
	{
		_tooltip?.ToggleDisplayStyle(visible: false);
	}

	private void UpdateToolVisibility()
	{
		Root.ToggleDisplayStyle(ToolEnabled);
	}
}
