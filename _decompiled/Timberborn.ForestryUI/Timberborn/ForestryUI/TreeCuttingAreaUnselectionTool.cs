using System.Collections.Generic;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Forestry;
using Timberborn.Localization;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using Timberborn.SelectionToolSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainQueryingSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using UnityEngine;

namespace Timberborn.ForestryUI;

internal class TreeCuttingAreaUnselectionTool : ITool, IToolDescriptor, ILoadableSingleton
{
	private static readonly string CursorKey = "CancelCursor";

	private static readonly string DescriptionLocKey = "TreeCuttingUnselectionTool.Description";

	private static readonly string TitleLocKey = "TreeCuttingUnselectionTool.Title";

	private readonly TreeCuttingArea _treeCuttingArea;

	private readonly TerrainAreaService _terrainAreaService;

	private readonly AreaHighlightingService _areaHighlightingService;

	private readonly IBlockService _blockService;

	private readonly SelectionToolProcessorFactory _selectionToolProcessorFactory;

	private readonly ILoc _loc;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly ISpecService _specService;

	private readonly MeasurableAreaDrawer _measurableAreaDrawer;

	private SelectionToolProcessor _selectionToolProcessor;

	private MeshDrawer _noActionMeshDrawer;

	private MeshDrawer _actionMeshDrawer;

	public TreeCuttingAreaUnselectionTool(TreeCuttingArea treeCuttingArea, TerrainAreaService terrainAreaService, AreaHighlightingService areaHighlightingService, IBlockService blockService, SelectionToolProcessorFactory selectionToolProcessorFactory, ILoc loc, MarkerDrawerFactory markerDrawerFactory, ISpecService specService, MeasurableAreaDrawer measurableAreaDrawer)
	{
		_treeCuttingArea = treeCuttingArea;
		_terrainAreaService = terrainAreaService;
		_areaHighlightingService = areaHighlightingService;
		_blockService = blockService;
		_selectionToolProcessorFactory = selectionToolProcessorFactory;
		_loc = loc;
		_markerDrawerFactory = markerDrawerFactory;
		_specService = specService;
		_measurableAreaDrawer = measurableAreaDrawer;
	}

	public void Load()
	{
		TreeCuttingColorsSpec singleSpec = _specService.GetSingleSpec<TreeCuttingColorsSpec>();
		_actionMeshDrawer = _markerDrawerFactory.CreatePrioritizedTileDrawer(singleSpec.ToolActionTile);
		_noActionMeshDrawer = _markerDrawerFactory.CreateTileDrawer(singleSpec.ToolNoActionTile);
		_selectionToolProcessor = _selectionToolProcessorFactory.Create(PreviewCallback, ActionCallback, ShowNoneCallback, CursorKey);
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
			_measurableAreaDrawer.AddMeasurableCoordinates(item);
			if (_treeCuttingArea.IsInCuttingArea(item))
			{
				_actionMeshDrawer.DrawAtCoordinates(item, 0.03f);
				TreeComponent bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<TreeComponent>(item);
				if (bottomObjectComponentAt != null)
				{
					_areaHighlightingService.AddForHighlight(bottomObjectComponentAt);
				}
			}
			else
			{
				_noActionMeshDrawer.DrawAtCoordinates(item, 0.02f);
			}
		}
		_areaHighlightingService.Highlight();
	}

	private void ActionCallback(IEnumerable<Vector3Int> inputBlocks, Ray ray)
	{
		_areaHighlightingService.UnhighlightAll();
		IEnumerable<Vector3Int> coordinates = _terrainAreaService.InMapLeveledCoordinates(inputBlocks, ray);
		_treeCuttingArea.RemoveCoordinates(coordinates);
	}

	private void ShowNoneCallback()
	{
		_areaHighlightingService.UnhighlightAll();
	}
}
