using System.Collections.Generic;
using System.Linq;
using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Localization;
using Timberborn.RecoverableGoodSystemUI;
using Timberborn.TerrainPhysics;
using Timberborn.TerrainSystemRendering;
using Timberborn.ToolSystemUI;
using Timberborn.UndoSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DeconstructionSystemUI;

public class BuildingDeconstructionTool : BlockObjectDeletionTool<BuildingSpec>
{
	private static readonly string DescriptionLocKey = "DeletionTool.Description.Buildings";

	private static readonly string ToolTitleLocKey = "DeletionTool.Title.Buildings";

	private readonly ILoc _loc;

	private readonly RecoverableGoodElementFactory _recoverableGoodElementFactory;

	private readonly RecoverableGoodTooltip _recoverableGoodTooltip;

	private readonly TerrainDestroyer _terrainDestroyer;

	private readonly HashSet<BlockObject> _objectsToDeconstruct = new HashSet<BlockObject>();

	private bool _previewDisabled;

	private readonly List<IBlockObjectDeletionBlocker> _deletionBlockers = new List<IBlockObjectDeletionBlocker>();

	protected override string ToolPromptLocKey => "DeletionTool.Prompt.Buildings";

	protected override string CursorKey => "DeleteBuildingCursor";

	public BuildingDeconstructionTool(InputService inputService, AreaBlockObjectAndTerrainPicker areaBlockObjectAndTerrainPicker, EntityService entityService, BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory, CursorService cursorService, ILoc loc, BlockObjectModelBlockadeIgnorer blockObjectModelBlockadeIgnorer, ISpecService specService, ILevelVisibilityService levelVisibilityService, DialogBoxShower dialogBoxShower, RecoverableGoodElementFactory recoverableGoodElementFactory, RecoverableGoodTooltip recoverableGoodTooltip, TerrainDestroyer terrainDestroyer, TerrainHighlightingService terrainHighlightingService, IUndoRegistry undoRegistry)
		: base(inputService, areaBlockObjectAndTerrainPicker, entityService, blockObjectSelectionDrawerFactory, cursorService, blockObjectModelBlockadeIgnorer, specService, levelVisibilityService, dialogBoxShower, terrainDestroyer, terrainHighlightingService, undoRegistry)
	{
		_loc = loc;
		_recoverableGoodElementFactory = recoverableGoodElementFactory;
		_recoverableGoodTooltip = recoverableGoodTooltip;
	}

	public void TogglePreview()
	{
		_previewDisabled = !_previewDisabled;
	}

	public override ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder(_loc.T(ToolTitleLocKey)).AddSection(_loc.T(DescriptionLocKey)).Build();
	}

	public override void Enter()
	{
		base.Enter();
		_recoverableGoodTooltip.Enable();
	}

	public override void Exit()
	{
		base.Exit();
		_recoverableGoodTooltip.Disable();
	}

	protected override void PostPreviewAction(IEnumerable<BlockObject> blockObjects)
	{
		_recoverableGoodTooltip.SetRecoverableGoods(blockObjects);
	}

	protected override VisualElement GetDialogBoxContent(IEnumerable<BlockObject> blockObjects)
	{
		return _recoverableGoodElementFactory.Create(blockObjects);
	}

	protected override bool IsBlockObjectValid(BlockObject blockObject)
	{
		return !IsStackedDeletionBlocked(blockObject);
	}

	protected override void PreviewCallback(IEnumerable<BlockObject> blockObjects, IEnumerable<Vector3Int> terrainBlocks, Vector3Int start, Vector3Int end, bool selectionStarted, bool selectingArea)
	{
		if (!_previewDisabled)
		{
			FillObjectsToDeconstruct(blockObjects);
			base.PreviewCallback(_objectsToDeconstruct, terrainBlocks, start, end, selectionStarted, selectingArea);
			_objectsToDeconstruct.Clear();
		}
	}

	private void FillObjectsToDeconstruct(IEnumerable<BlockObject> blockObjects)
	{
		foreach (BlockObject blockObject in blockObjects)
		{
			_objectsToDeconstruct.Add(blockObject);
			IRecoverableObjectAdder component = blockObject.GetComponent<IRecoverableObjectAdder>();
			if (component != null)
			{
				BlockObject additionalObjectToRecover = component.GetAdditionalObjectToRecover();
				if ((bool)additionalObjectToRecover)
				{
					_objectsToDeconstruct.Add(additionalObjectToRecover);
				}
			}
		}
	}

	private bool IsStackedDeletionBlocked(BlockObject blockObject)
	{
		blockObject.GetComponents(_deletionBlockers);
		bool result = _deletionBlockers.Any((IBlockObjectDeletionBlocker blocker) => blocker.IsStackedDeletionBlocked);
		_deletionBlockers.Clear();
		return result;
	}
}
