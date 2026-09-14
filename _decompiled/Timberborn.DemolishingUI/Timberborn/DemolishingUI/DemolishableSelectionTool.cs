using System.Collections.Generic;
using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Demolishing;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.Planting;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using UnityEngine;

namespace Timberborn.DemolishingUI;

public class DemolishableSelectionTool : ITool, IToolDescriptor, ILoadableSingleton, IInputProcessor
{
	private static readonly string CursorKey = "DemolishResourcesCursor";

	private static readonly string TitleLocKey = "DemolishSelectionTool.Title";

	private static readonly string DescriptionLocKey = "DemolishSelectionTool.Description";

	private readonly ILoc _loc;

	private readonly ISpecService _specService;

	private readonly InputService _inputService;

	private readonly CursorService _cursorService;

	private readonly PlantingService _plantingService;

	private readonly ITerrainService _terrainService;

	private readonly AreaBlockObjectPickerFactory _areaBlockObjectPickerFactory;

	private readonly BlockObjectSelectionDrawerFactory _blockObjectSelectionDrawerFactory;

	private BlockObjectSelectionDrawer _blockObjectSelectionDrawer;

	private AreaBlockObjectPicker _areaBlockObjectPicker;

	public DemolishableSelectionTool(ILoc loc, ISpecService specService, InputService inputService, CursorService cursorService, PlantingService plantingService, ITerrainService terrainService, AreaBlockObjectPickerFactory areaBlockObjectPickerFactory, BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory)
	{
		_loc = loc;
		_specService = specService;
		_inputService = inputService;
		_cursorService = cursorService;
		_plantingService = plantingService;
		_terrainService = terrainService;
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
			blockObject.GetComponent<Demolishable>().Mark();
		}
		UnsetPlantingCoordinates(start, end);
	}

	private void ShowNoneCallback()
	{
		_blockObjectSelectionDrawer.StopDrawing();
	}

	private void UnsetPlantingCoordinates(Vector3Int start, Vector3Int end)
	{
		(Vector3Int min, Vector3Int max) tuple = Vectors.MinMax(start, end);
		Vector3Int item = tuple.min;
		Vector3Int item2 = tuple.max;
		int z = start.z;
		for (int i = item.x; i <= item2.x; i++)
		{
			for (int j = item.y; j <= item2.y; j++)
			{
				Vector3Int coordinates = new Vector3Int(i, j, z);
				if (_terrainService.Contains(coordinates) && _terrainService.GetTerrainHeightBelow(coordinates) == z)
				{
					_plantingService.UnsetPlantingCoordinates(coordinates);
				}
			}
		}
	}
}
