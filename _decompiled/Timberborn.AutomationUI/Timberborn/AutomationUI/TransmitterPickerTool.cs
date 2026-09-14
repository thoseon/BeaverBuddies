using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.ConstructionMode;
using Timberborn.CursorToolSystem;
using Timberborn.DropdownSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UISound;

namespace Timberborn.AutomationUI;

public class TransmitterPickerTool : ITool, IInputProcessor, IToolDescriptor, IConstructionModeEnabler, IGroupIgnoringTool
{
	private static readonly string DescriptionLocKey = "Automation.SelectTransmitter";

	private static readonly string CursorKey = "PickObjectCursor";

	private readonly InputService _inputService;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	private readonly ToolService _toolService;

	private readonly CursorService _cursorService;

	private readonly CursorTool _cursorTool;

	private readonly ILoc _loc;

	private readonly UISoundController _uiSoundController;

	private readonly EventBus _eventBus;

	private readonly DropdownListDrawer _dropdownListDrawer;

	private readonly TransmitterPickerToolHighlighter _transmitterPickerToolHighlighter;

	private readonly AutomatorRegistry _automatorRegistry;

	private BaseComponent _owner;

	private Dropdown _dropdown;

	private Action<Automator> _setter;

	internal TransmitterPickerTool(InputService inputService, SelectableObjectRaycaster selectableObjectRaycaster, ToolService toolService, CursorService cursorService, CursorTool cursorTool, ILoc loc, UISoundController uiSoundController, EventBus eventBus, DropdownListDrawer dropdownListDrawer, TransmitterPickerToolHighlighter transmitterPickerToolHighlighter, AutomatorRegistry automatorRegistry)
	{
		_inputService = inputService;
		_selectableObjectRaycaster = selectableObjectRaycaster;
		_toolService = toolService;
		_cursorService = cursorService;
		_cursorTool = cursorTool;
		_loc = loc;
		_uiSoundController = uiSoundController;
		_eventBus = eventBus;
		_dropdownListDrawer = dropdownListDrawer;
		_transmitterPickerToolHighlighter = transmitterPickerToolHighlighter;
		_automatorRegistry = automatorRegistry;
	}

	public void SwitchTo(BaseComponent owner, Dropdown dropdown, Action<Automator> setter)
	{
		Asserts.FieldIsNull(this, _owner, "_owner");
		_owner = owner;
		_dropdown = dropdown;
		_setter = setter;
		_cursorTool.DisableNextExitUnselect();
		_toolService.SwitchTool(this);
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
		_eventBus.Register(this);
		_dropdownListDrawer.IgnoreWorldInput(ignoreWorldInput: true);
		_transmitterPickerToolHighlighter.Highlight(_owner);
		_cursorService.SetCursor(CursorKey);
	}

	public void Exit()
	{
		_inputService.RemoveInputProcessor(this);
		_eventBus.Unregister(this);
		_dropdownListDrawer.IgnoreWorldInput(ignoreWorldInput: false);
		_cursorService.ResetCursor();
		_transmitterPickerToolHighlighter.Clear();
		_owner = null;
		_setter = null;
		_dropdown.Hide();
		_dropdown = null;
	}

	public bool ProcessInput()
	{
		if (_toolService.ActiveTool == this)
		{
			Automator hoveredTransmitter = GetHoveredTransmitter();
			_transmitterPickerToolHighlighter.UpdateHover(hoveredTransmitter);
			if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
			{
				if ((bool)hoveredTransmitter)
				{
					_uiSoundController.PlayClickSound();
					_setter(hoveredTransmitter);
					_dropdown.UpdateSelectedValue();
					_toolService.SwitchToDefaultTool();
					return true;
				}
				_uiSoundController.PlayCantDoSound();
			}
		}
		return false;
	}

	public ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder().AddPrioritizedSection(_loc.T(DescriptionLocKey)).Build();
	}

	[OnEvent]
	public void OnDropdownHidden(DropdownHiddenEvent dropdownHiddenEvent)
	{
		_toolService.SwitchToDefaultTool();
	}

	private Automator GetHoveredTransmitter()
	{
		if (_selectableObjectRaycaster.TryHitSelectableObject(out var hitObject))
		{
			Automator component = hitObject.GetComponent<Automator>();
			if (component != null && component.IsTransmitter)
			{
				return component;
			}
		}
		return GetHoveredTransmitterOnDropdown();
	}

	private Automator GetHoveredTransmitterOnDropdown()
	{
		if (!string.IsNullOrEmpty(_dropdown.HoveredItem))
		{
			Guid entityId = Guid.Parse(_dropdown.HoveredItem);
			return _automatorRegistry.FindTransmitterById(entityId);
		}
		return null;
	}
}
