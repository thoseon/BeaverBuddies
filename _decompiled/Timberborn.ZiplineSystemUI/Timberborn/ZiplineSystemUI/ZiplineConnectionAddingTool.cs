using Timberborn.Common;
using Timberborn.ConstructionMode;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UISound;
using Timberborn.ZiplineSystem;

namespace Timberborn.ZiplineSystemUI;

internal class ZiplineConnectionAddingTool : ITool, IToolDescriptor, IInputProcessor, IConstructionModeEnabler
{
	private static readonly string DescriptionLocKey = "Zipline.PickDestination";

	private static readonly string CursorKey = "PickObjectCursor";

	private readonly InputService _inputService;

	private readonly SelectableObjectRaycaster _selectableObjectRaycaster;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly ToolService _toolService;

	private readonly ZiplineConnectionService _ziplineConnectionService;

	private readonly CursorService _cursorService;

	private readonly ILoc _loc;

	private readonly ZiplinePreviewCableRenderer _ziplinePreviewCableRenderer;

	private readonly UISoundController _uiSoundController;

	private readonly ConnectionCandidates _connectionCandidates;

	private ZiplineTower _currentZiplineTower;

	public ZiplineConnectionAddingTool(InputService inputService, SelectableObjectRaycaster selectableObjectRaycaster, EntitySelectionService entitySelectionService, ToolService toolService, ZiplineConnectionService ziplineConnectionService, CursorService cursorService, ILoc loc, ZiplinePreviewCableRenderer ziplinePreviewCableRenderer, UISoundController uiSoundController, ConnectionCandidates connectionCandidates)
	{
		_inputService = inputService;
		_selectableObjectRaycaster = selectableObjectRaycaster;
		_entitySelectionService = entitySelectionService;
		_toolService = toolService;
		_ziplineConnectionService = ziplineConnectionService;
		_cursorService = cursorService;
		_loc = loc;
		_ziplinePreviewCableRenderer = ziplinePreviewCableRenderer;
		_uiSoundController = uiSoundController;
		_connectionCandidates = connectionCandidates;
	}

	public void Enter()
	{
		Asserts.FieldIsNotNull(this, _currentZiplineTower, "_currentZiplineTower");
		_connectionCandidates.Enable(_currentZiplineTower);
		_inputService.AddInputProcessor(this);
		_cursorService.SetCursor(CursorKey);
	}

	public void Exit()
	{
		_connectionCandidates.Disable();
		_ziplinePreviewCableRenderer.HidePreview();
		_inputService.RemoveInputProcessor(this);
		_cursorService.ResetCursor();
		_entitySelectionService.Select(_currentZiplineTower);
		_currentZiplineTower = null;
	}

	public ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder().AddPrioritizedSection(_loc.T(DescriptionLocKey)).Build();
	}

	public bool ProcessInput()
	{
		ZiplineTower ziplineTower = (_selectableObjectRaycaster.TryHitSelectableObject(out var hitObject) ? hitObject.GetComponent<ZiplineTower>() : null);
		if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
		{
			if (_ziplineConnectionService.CanBeConnected(_currentZiplineTower, ziplineTower))
			{
				Connect(ziplineTower);
				_uiSoundController.PlayClickSound();
				return true;
			}
			_uiSoundController.PlayCantDoSound();
		}
		_ziplinePreviewCableRenderer.DrawPreview(_currentZiplineTower, ziplineTower, _connectionCandidates.Contains(ziplineTower));
		return false;
	}

	public void SwitchTo(ZiplineTower ziplineTower)
	{
		_currentZiplineTower = ziplineTower;
	}

	private void Connect(ZiplineTower ziplineTower)
	{
		_ziplineConnectionService.Connect(_currentZiplineTower, ziplineTower);
		_toolService.SwitchToDefaultTool();
		_entitySelectionService.Select(ziplineTower);
		if (ziplineTower.HasFreeSlots)
		{
			SwitchTo(ziplineTower);
			_toolService.SwitchTool(this);
		}
	}
}
