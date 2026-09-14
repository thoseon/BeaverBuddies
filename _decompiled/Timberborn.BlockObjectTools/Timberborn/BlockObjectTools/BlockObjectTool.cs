using System.Collections.Generic;
using Timberborn.AreaSelectionSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.ConstructionGuidelines;
using Timberborn.ConstructionMode;
using Timberborn.Coordinates;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UISound;
using Timberborn.UndoSystem;

namespace Timberborn.BlockObjectTools;

public class BlockObjectTool : IDevModeTool, ITool, IToolDescriptor, IInputProcessor, IConstructionModeEnabler, IBlockObjectGridTool
{
	private static readonly string BlockObjectPlacedSoundName = "UI.BlockObjectPlaced";

	private readonly InputService _inputService;

	private readonly ToolService _toolService;

	private readonly UISoundController _uiSoundController;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly IBlockObjectPlacer _blockObjectPlacer;

	private readonly IBlockObjectToolDescriber _blockObjectToolDescriber;

	private readonly PreviewPlacer _previewPlacer;

	private readonly IUndoRegistry _undoRegistry;

	private readonly PreviewPlacement _previewPlacement;

	private readonly AreaPicker _areaPicker;

	private bool _placedAnythingThisFrame;

	private BaseComponent _duplicationSource;

	public PlaceableBlockObjectSpec Template { get; }

	public bool IsDevMode => Template.DevModeTool;

	public string WarningText => _previewPlacer.WarningText;

	public BlockObjectTool(PlaceableBlockObjectSpec template, ToolService toolService, InputService inputService, AreaPicker areaPicker, UISoundController uiSoundController, ToolUnlockingService toolUnlockingService, IBlockObjectPlacer blockObjectPlacer, IBlockObjectToolDescriber blockObjectToolDescriber, PreviewPlacer previewPlacer, IUndoRegistry undoRegistry, PreviewPlacement previewPlacement)
	{
		Template = template;
		_toolService = toolService;
		_inputService = inputService;
		_areaPicker = areaPicker;
		_uiSoundController = uiSoundController;
		_toolUnlockingService = toolUnlockingService;
		_blockObjectPlacer = blockObjectPlacer;
		_blockObjectToolDescriber = blockObjectToolDescriber;
		_previewPlacer = previewPlacer;
		_undoRegistry = undoRegistry;
		_previewPlacement = previewPlacement;
	}

	public bool ProcessInput()
	{
		return _areaPicker.PickBlockObjectArea(Template, _previewPlacement.Orientation, _previewPlacement.FlipMode, PreviewCallback, ActionCallback);
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
	}

	public void Exit()
	{
		_inputService.RemoveInputProcessor(this);
		_previewPlacer.HideAllPreviews();
		_areaPicker.Reset();
		_placedAnythingThisFrame = false;
		_duplicationSource = null;
	}

	public ToolDescription DescribeTool()
	{
		return _blockObjectToolDescriber.Describe(this, _blockObjectPlacer);
	}

	public void ActivateWithDuplicationSource(BaseComponent value)
	{
		_duplicationSource = value;
		_previewPlacement.CopyFrom(value.GetComponent<BlockObject>());
		_toolService.SwitchTool(this);
	}

	private void PreviewCallback(IEnumerable<Placement> placements)
	{
		if (_placedAnythingThisFrame)
		{
			_placedAnythingThisFrame = false;
		}
		else
		{
			ShowPreviews(placements);
		}
	}

	private void ActionCallback(IEnumerable<Placement> placements)
	{
		if (_toolUnlockingService.IsLocked(this))
		{
			_toolUnlockingService.TryToUnlock(this, delegate
			{
				Place(placements);
			}, _previewPlacer.HideAllPreviews);
		}
		else
		{
			Place(placements);
		}
	}

	private void ShowPreviews(IEnumerable<Placement> placements)
	{
		_previewPlacer.ShowPreviews(placements);
	}

	private void Place(IEnumerable<Placement> placements)
	{
		_placedAnythingThisFrame = false;
		foreach (Placement buildableCoordinate in _previewPlacer.GetBuildableCoordinates(placements))
		{
			EntitySetup.Builder builder = new EntitySetup.Builder(Template.Blueprint);
			if ((bool)_duplicationSource)
			{
				builder.AddInitComponent(new DuplicationInit(_duplicationSource));
			}
			_blockObjectPlacer.Place(builder, buildableCoordinate);
			_placedAnythingThisFrame = true;
		}
		_undoRegistry.CommitStack();
		if (_placedAnythingThisFrame)
		{
			_uiSoundController.PlaySound(BlockObjectPlacedSoundName);
			_previewPlacer.HideAllPreviews();
		}
		else
		{
			_uiSoundController.PlayCantDoSound();
		}
	}
}
