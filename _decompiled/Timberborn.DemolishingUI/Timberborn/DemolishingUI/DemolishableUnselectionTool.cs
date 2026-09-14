using System.Collections.Generic;
using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Demolishing;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using UnityEngine;

namespace Timberborn.DemolishingUI;

public class DemolishableUnselectionTool : ITool, IToolDescriptor, ILoadableSingleton, IInputProcessor
{
	private static readonly string CursorKey = "CancelCursor";

	private static readonly string TitleLocKey = "DemolishUnselectionTool.Title";

	private static readonly string DescriptionLocKey = "DemolishUnselectionTool.Description";

	private readonly ILoc _loc;

	private readonly ISpecService _specService;

	private readonly InputService _inputService;

	private readonly CursorService _cursorService;

	private readonly AreaBlockObjectPickerFactory _areaBlockObjectPickerFactory;

	private readonly BlockObjectSelectionDrawerFactory _blockObjectSelectionDrawerFactory;

	private BlockObjectSelectionDrawer _blockObjectSelectionDrawer;

	private AreaBlockObjectPicker _areaBlockObjectPicker;

	public DemolishableUnselectionTool(ILoc loc, ISpecService specService, InputService inputService, CursorService cursorService, AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory)
	{
		_loc = loc;
		_specService = specService;
		_inputService = inputService;
		_cursorService = cursorService;
		_areaBlockObjectPickerFactory = areaBlockObjectPickerFactory;
		_blockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
	}

	public void Load()
	{
		DemolishingColorsSpec singleSpec = _specService.GetSingleSpec<DemolishingColorsSpec>();
		_blockObjectSelectionDrawer = _blockObjectSelectionDrawerFactory.Create(singleSpec.DeletedObjectHighlightColor, singleSpec.DeletedAreaTileColor, singleSpec.DeletedAreaSideColor);
		_areaBlockObjectPicker = _areaBlockObjectPickerFactory.CreatePickingUpwards();
	}

	public ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder(_loc.T(TitleLocKey)).AddSection(_loc.T(DescriptionLocKey)).Build();
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
		_cursorService.SetCursor(CursorKey);
	}

	public void Exit()
	{
		_blockObjectSelectionDrawer.StopDrawing();
		_areaBlockObjectPicker.Reset();
		_inputService.RemoveInputProcessor(this);
		_cursorService.ResetCursor();
		ShowNoneCallback();
	}

	public bool ProcessInput()
	{
		return _areaBlockObjectPicker.PickBlockObjects<Demolishable>(PreviewCallback, ActionCallback, ShowNoneCallback);
	}

	private void PreviewCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea)
	{
		_blockObjectSelectionDrawer.Draw(blockObjects, start, end, selectingArea);
	}

	private void ActionCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea)
	{
		foreach (BlockObject blockObject in blockObjects)
		{
			blockObject.GetComponent<Demolishable>().Unmark();
		}
	}

	private void ShowNoneCallback()
	{
		_blockObjectSelectionDrawer.StopDrawing();
	}
}
