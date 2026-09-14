using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.ConstructionMode;
using Timberborn.DuplicationSystem;
using Timberborn.EntityUndoSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UISound;

namespace Timberborn.DuplicationSystemUI;

internal class DuplicateSettingsTool : ITool, IToolDescriptor, IConstructionModeEnabler, ILoadableSingleton, IInputProcessor
{
	private static readonly string DescriptionLocKey = "Duplication.DuplicateSettingsToolDescription";

	private static readonly string CursorKey = "DuplicateSettingsCursor";

	private readonly InputService _inputService;

	private readonly ToolService _toolService;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	private readonly Highlighter _highlighter;

	private readonly RollingHighlighter _rollingHighlighter;

	private readonly ISpecService _specService;

	private readonly Duplicator _duplicator;

	private readonly CursorService _cursorService;

	private readonly ILoc _loc;

	private readonly UISoundController _uiSoundController;

	private readonly EntityChangeRecorderFactory _entityChangeRecorderFactory;

	private readonly DuplicationValidator _duplicationValidator;

	private DuplicationSystemColorsSpec _duplicationSystemColorsSpec;

	private BaseComponent _source;

	private BaseComponent _lastTarget;

	public DuplicateSettingsTool(InputService inputService, ToolService toolService, SelectableObjectRaycaster selectableObjectRaycaster, Highlighter highlighter, RollingHighlighter rollingHighlighter, ISpecService specService, Duplicator duplicator, CursorService cursorService, ILoc loc, UISoundController uiSoundController, EntityChangeRecorderFactory entityChangeRecorderFactory, DuplicationValidator duplicationValidator)
	{
		_inputService = inputService;
		_toolService = toolService;
		_selectableObjectRaycaster = selectableObjectRaycaster;
		_highlighter = highlighter;
		_rollingHighlighter = rollingHighlighter;
		_specService = specService;
		_duplicator = duplicator;
		_cursorService = cursorService;
		_loc = loc;
		_uiSoundController = uiSoundController;
		_entityChangeRecorderFactory = entityChangeRecorderFactory;
		_duplicationValidator = duplicationValidator;
	}

	public void Load()
	{
		_duplicationSystemColorsSpec = _specService.GetSingleSpec<DuplicationSystemColorsSpec>();
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
		_cursorService.SetCursor(CursorKey);
		_highlighter.HighlightPrimary(_source, _duplicationSystemColorsSpec.SourceColor);
	}

	public void Exit()
	{
		_source = null;
		_inputService.RemoveInputProcessor(this);
		_cursorService.ResetCursor();
		_highlighter.UnhighlightAllPrimary();
		_rollingHighlighter.UnhighlightAllPrimary();
	}

	public ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder().AddPrioritizedSection(_loc.T(DescriptionLocKey)).Build();
	}

	public bool ProcessInput()
	{
		if (!_source)
		{
			_toolService.SwitchToDefaultTool();
			return true;
		}
		if (_selectableObjectRaycaster.TryHitSelectableObject(out var hitObject) && _duplicationValidator.CanDuplicateSettings(hitObject))
		{
			if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
			{
				DuplicateTo(hitObject);
				_uiSoundController.PlayClickSound();
				return true;
			}
			if (_inputService.MainMouseButtonHeld)
			{
				_rollingHighlighter.UnhighlightAllPrimary();
			}
			else
			{
				_rollingHighlighter.HighlightPrimary(hitObject, _duplicationSystemColorsSpec.TargetColor);
			}
		}
		else
		{
			_rollingHighlighter.UnhighlightAllPrimary();
		}
		return false;
	}

	public void ActivateWithSource(BaseComponent source)
	{
		_source = source;
		_toolService.SwitchTool(this);
	}

	private void DuplicateTo(BaseComponent target)
	{
		using (_entityChangeRecorderFactory.CreateChangeRecorder(target))
		{
			_duplicator.Duplicate(_source, target);
		}
	}
}
