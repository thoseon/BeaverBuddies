using System.Collections.Generic;
using System.Linq;
using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.Common;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.PrioritySystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UISound;
using UnityEngine;

namespace Timberborn.BuilderPrioritySystemUI;

internal class BuilderPriorityTool : ITool, IToolDescriptor, IInputProcessor
{
	private static readonly string CursorKey = "PriorityCursor";

	private static readonly string DescriptionKey = "BuilderPriorityTool.Description";

	private static readonly string TipKey = "BuilderPriorityTool.Tip";

	private readonly AreaBlockObjectPickerFactory _areaBlockObjectPickerFactory;

	private readonly InputService _inputService;

	private readonly BlockObjectSelectionDrawerFactory _blockObjectSelectionDrawerFactory;

	private readonly CursorService _cursorService;

	private readonly ILoc _loc;

	private readonly BuilderPrioritizableHighlighter _builderPrioritizableHighlighter;

	private readonly UISoundController _uiSoundController;

	private BlockObjectSelectionDrawer _highlightSelectionDrawer;

	private BlockObjectSelectionDrawer _actionSelectionDrawer;

	private AreaBlockObjectPicker _areaBlockObjectPicker;

	private ToolDescription _toolDescription;

	private Priority _priority;

	private readonly List<(BlockObject, BuilderPrioritizable)> _builderPrioritizables = new List<(BlockObject, BuilderPrioritizable)>();

	public BuilderPriorityTool(AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, InputService inputService, BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory, CursorService cursorService, ILoc loc, BuilderPrioritizableHighlighter builderPrioritizableHighlighter, UISoundController uiSoundController)
	{
		_areaBlockObjectPickerFactory = areaBlockObjectPickerFactory;
		_inputService = inputService;
		_blockObjectSelectionDrawerFactory = blockObjectSelectionDrawerFactory;
		_cursorService = cursorService;
		_loc = loc;
		_builderPrioritizableHighlighter = builderPrioritizableHighlighter;
		_uiSoundController = uiSoundController;
	}

	public void Initialize(Priority priority, BuilderPriorityToolSpec spec)
	{
		_priority = priority;
		_areaBlockObjectPicker = _areaBlockObjectPickerFactory.CreatePickingDownwards();
		_highlightSelectionDrawer = _blockObjectSelectionDrawerFactory.Create(spec.PriorityHighlightColor, spec.PriorityTileColor, spec.PrioritySideColor);
		_actionSelectionDrawer = _blockObjectSelectionDrawerFactory.Create(spec.PriorityActionColor, spec.PriorityTileColor, spec.PrioritySideColor);
		InitializeToolDescription();
	}

	public bool ProcessInput()
	{
		return _areaBlockObjectPicker.PickBlockObjects<BuilderPrioritizable>(PreviewCallback, ActionCallback, ShowNoneCallback);
	}

	public void Enter()
	{
		_inputService.AddInputProcessor(this);
		_cursorService.SetCursor(CursorKey);
	}

	public void Exit()
	{
		_cursorService.ResetCursor();
		_areaBlockObjectPicker.Reset();
		_highlightSelectionDrawer.StopDrawing();
		_actionSelectionDrawer.StopDrawing();
		_inputService.RemoveInputProcessor(this);
	}

	public ToolDescription DescribeTool()
	{
		return _toolDescription;
	}

	private void PreviewCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea)
	{
		IEnumerable<BlockObject> blockObjects2 = blockObjects.Where((BlockObject bo) => bo.GetComponent<BuilderPrioritizable>()?.Enabled ?? false);
		if (selectionStarted && !selectingArea)
		{
			_actionSelectionDrawer.Draw(blockObjects2, start, end, selectingArea: false);
		}
		else if (selectingArea)
		{
			_actionSelectionDrawer.Draw(blockObjects2, start, end, selectingArea: true);
		}
		else
		{
			_highlightSelectionDrawer.Draw(blockObjects2, start, end, selectingArea: false);
		}
	}

	private void ActionCallback(IEnumerable<BlockObject> blockObjects, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea)
	{
		IEnumerable<(BlockObject, BuilderPrioritizable)> collection = from bo in blockObjects
			select (bo: bo, bo.GetComponent<BuilderPrioritizable>()) into tuple
			where (bool)tuple.Item2 && tuple.Item2.Enabled
			select tuple;
		_builderPrioritizables.AddRange(collection);
		foreach (var builderPrioritizable in _builderPrioritizables)
		{
			builderPrioritizable.Item2.SetPriority(_priority);
		}
		if (!_builderPrioritizables.IsEmpty())
		{
			_builderPrioritizableHighlighter.HighlightAll();
			_actionSelectionDrawer.Draw(_builderPrioritizables.Select(((BlockObject, BuilderPrioritizable) tuple) => tuple.Item1), start, end, selectingArea);
			_uiSoundController.PlayClickSound();
		}
		_builderPrioritizables.Clear();
		ClearHighlights();
	}

	private void ShowNoneCallback()
	{
		ClearHighlights();
	}

	private void ClearHighlights()
	{
		_highlightSelectionDrawer.StopDrawing();
		_actionSelectionDrawer.StopDrawing();
	}

	private void InitializeToolDescription()
	{
		string title = _loc.T("Priorities." + _priority);
		_toolDescription = new ToolDescription.Builder(title).AddSection(_loc.T(DescriptionKey)).AddPrioritizedSection(_loc.T(TipKey)).Build();
	}
}
