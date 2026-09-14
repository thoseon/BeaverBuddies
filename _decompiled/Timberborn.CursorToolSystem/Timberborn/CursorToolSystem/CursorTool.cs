using Timberborn.InputSystem;
using Timberborn.Options;
using Timberborn.SelectionSystem;
using Timberborn.ToolSystem;
using Timberborn.UILayoutSystem;

namespace Timberborn.CursorToolSystem;

public class CursorTool : ITool, IInputProcessor
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly InputService _inputService;

	private readonly IOptionsBox _optionsBox;

	private readonly UIVisibilityManager _uiVisibilityManager;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	private bool _disableNextExitUnselect;

	public CursorTool(EntitySelectionService entitySelectionService, InputService inputService, IOptionsBox optionsBox, UIVisibilityManager uiVisibilityManager, SelectableObjectRaycaster selectableObjectRaycaster)
	{
		_entitySelectionService = entitySelectionService;
		_inputService = inputService;
		_optionsBox = optionsBox;
		_uiVisibilityManager = uiVisibilityManager;
		_selectableObjectRaycaster = selectableObjectRaycaster;
	}

	public bool ProcessInput()
	{
		if (!ProcessSelectObject() && !ProcessUnselectObject())
		{
			return ProcessShowOptions();
		}
		return true;
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
		_disableNextExitUnselect = false;
	}

	public void Exit()
	{
		if (_disableNextExitUnselect)
		{
			_disableNextExitUnselect = false;
		}
		else
		{
			_entitySelectionService.Unselect();
		}
		_inputService.RemoveInputProcessor(this);
	}

	public void DisableNextExitUnselect()
	{
		_disableNextExitUnselect = true;
	}

	private bool ProcessSelectObject()
	{
		if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
		{
			if (_selectableObjectRaycaster.TryHitSelectableObjectIncludeTerrainStump(out var hitObject))
			{
				_entitySelectionService.Select(hitObject);
			}
			else
			{
				_entitySelectionService.Unselect();
			}
			return true;
		}
		return false;
	}

	private bool ProcessUnselectObject()
	{
		if (_entitySelectionService.IsAnythingSelected && _inputService.Cancel)
		{
			_entitySelectionService.Unselect();
			return true;
		}
		return false;
	}

	private bool ProcessShowOptions()
	{
		if (_inputService.UICancel && _uiVisibilityManager.GUIVisible)
		{
			_optionsBox.Show();
			return true;
		}
		return false;
	}
}
