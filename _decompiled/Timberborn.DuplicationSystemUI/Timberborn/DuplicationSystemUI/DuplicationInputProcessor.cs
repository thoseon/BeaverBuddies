using Timberborn.InputSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.DuplicationSystemUI;

internal class DuplicationInputProcessor : ILoadableSingleton, IUnloadableSingleton, IInputProcessor
{
	private static readonly string DuplicateSettingsKey = "DuplicateSettings";

	private static readonly string DuplicateObjectKey = "DuplicateObject";

	private readonly InputService _inputService;

	private readonly ToolService _toolService;

	private readonly DuplicateSettingsTool _duplicateSettingsTool;

	private readonly DuplicationValidator _duplicationValidator;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	public DuplicationInputProcessor(InputService inputService, ToolService toolService, DuplicateSettingsTool duplicateSettingsTool, DuplicationValidator duplicationValidator, EntitySelectionService entitySelectionService, SelectableObjectRaycaster selectableObjectRaycaster)
	{
		_inputService = inputService;
		_toolService = toolService;
		_duplicateSettingsTool = duplicateSettingsTool;
		_duplicationValidator = duplicationValidator;
		_entitySelectionService = entitySelectionService;
		_selectableObjectRaycaster = selectableObjectRaycaster;
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public void Unload()
	{
		_inputService.RemoveInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_toolService.IsDefaultToolActive && !_entitySelectionService.IsAnythingSelected)
		{
			if (_inputService.IsKeyDown(DuplicateSettingsKey) && _selectableObjectRaycaster.TryHitSelectableObject(out var hitObject) && _duplicationValidator.CanDuplicateSettings(hitObject))
			{
				_duplicateSettingsTool.ActivateWithSource(hitObject);
				return true;
			}
			if (_inputService.IsKeyDown(DuplicateObjectKey) && _selectableObjectRaycaster.TryHitSelectableObject(out var hitObject2) && _duplicationValidator.CanDuplicateObject(hitObject2, out var toolActivationAction))
			{
				toolActivationAction();
				return true;
			}
		}
		return false;
	}
}
