using System.Collections.Generic;
using Timberborn.Localization;
using Timberborn.SelectionToolSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using UnityEngine;

namespace Timberborn.PlantingUI;

public class CancelPlantingTool : ITool, IToolDescriptor
{
	private static readonly string CursorKey = "CancelCursor";

	private static readonly string TitleLocKey = "CancelPlantingTool.Title";

	private static readonly string DescriptionLocKey = "CancelPlantingTool.Description";

	private readonly PlantingSelectionService _plantingSelectionService;

	private readonly ILoc _loc;

	private readonly SelectionToolProcessor _selectionToolProcessor;

	public CancelPlantingTool(PlantingSelectionService plantingSelectionService, ILoc loc, SelectionToolProcessorFactory selectionToolProcessorFactory)
	{
		_plantingSelectionService = plantingSelectionService;
		_loc = loc;
		_selectionToolProcessor = selectionToolProcessorFactory.Create(PreviewCallback, ActionCallback, ShowNoneCallback, CursorKey);
	}

	public void Enter()
	{
		_selectionToolProcessor.Enter();
	}

	public void Exit()
	{
		_plantingSelectionService.UnhighlightAll();
		_selectionToolProcessor.Exit();
	}

	public ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder(_loc.T(TitleLocKey)).AddSection(_loc.T(DescriptionLocKey)).Build();
	}

	private void PreviewCallback(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		_plantingSelectionService.HighlightUnmarkableArea(inputBlocks, ray);
	}

	private void ActionCallback(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		_plantingSelectionService.UnmarkArea(inputBlocks, ray);
	}

	private void ShowNoneCallback()
	{
		_plantingSelectionService.UnhighlightAll();
	}
}
