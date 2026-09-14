using System.Collections.Generic;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Forestry;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SelectionToolSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using UnityEngine;

namespace Timberborn.ForestryUI;

public class TreeCuttingAreaSelectionTool : ITool, IToolDescriptor, ILoadableSingleton
{
	private static readonly string CursorKey = "CutTreeCursor";

	private static readonly string DescriptionLocKey = "TreeCuttingSelectionTool.Description";

	private static readonly string TitleLocKey = "TreeCuttingSelectionTool.Title";

	private readonly TreeCuttingArea _treeCuttingArea;

	private readonly TerrainAreaService _terrainAreaService;

	private readonly AreaHighlightingService _areaHighlightingService;

	private readonly IBlockService _blockService;

	private readonly SelectionToolProcessorFactory _selectionToolProcessorFactory;

	private readonly ILoc _loc;

	private readonly ISpecService _specService;

	private readonly MeasurableAreaDrawer _measurableAreaDrawer;

	private SelectionToolProcessor _selectionToolProcessor;

	private Color _toolActionTileColor;

	public TreeCuttingAreaSelectionTool(TreeCuttingArea treeCuttingArea, TerrainAreaService terrainAreaService, AreaHighlightingService areaHighlightingService, IBlockService blockService, SelectionToolProcessorFactory selectionToolProcessorFactory, ILoc loc, ISpecService specService, MeasurableAreaDrawer measurableAreaDrawer)
	{
		_treeCuttingArea = treeCuttingArea;
		_terrainAreaService = terrainAreaService;
		_areaHighlightingService = areaHighlightingService;
		_blockService = blockService;
		_selectionToolProcessorFactory = selectionToolProcessorFactory;
		_loc = loc;
		_specService = specService;
		_measurableAreaDrawer = measurableAreaDrawer;
	}

	public void Load()
	{
		_selectionToolProcessor = _selectionToolProcessorFactory.Create(PreviewCallback, ActionCallback, ShowNoneCallback, CursorKey);
		_toolActionTileColor = _specService.GetSingleSpec<TreeCuttingColorsSpec>().ToolActionTile;
	}

	public ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder(_loc.T(TitleLocKey)).AddSection(_loc.T(DescriptionLocKey)).Build();
	}

	public void Enter()
	{
		_selectionToolProcessor.Enter();
	}

	public void Exit()
	{
		_areaHighlightingService.UnhighlightAll();
		_selectionToolProcessor.Exit();
	}

	private void PreviewCallback(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		foreach (Vector3Int item in _terrainAreaService.InMapLeveledCoordinates(inputBlocks, ray))
		{
			if (!_treeCuttingArea.IsInCuttingArea(item))
			{
				_areaHighlightingService.DrawTile(item, _toolActionTileColor);
				_measurableAreaDrawer.AddMeasurableCoordinates(item);
				TreeComponent bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<TreeComponent>(item);
				if (bottomObjectComponentAt != null)
				{
					_areaHighlightingService.AddForHighlight(bottomObjectComponentAt);
				}
			}
		}
		_areaHighlightingService.Highlight();
	}

	private void ActionCallback(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		_areaHighlightingService.UnhighlightAll();
		IEnumerable<Vector3Int> coordinates = _terrainAreaService.InMapLeveledCoordinates(inputBlocks, ray);
		_treeCuttingArea.AddCoordinates(coordinates);
	}

	private void ShowNoneCallback()
	{
		_areaHighlightingService.UnhighlightAll();
	}
}
