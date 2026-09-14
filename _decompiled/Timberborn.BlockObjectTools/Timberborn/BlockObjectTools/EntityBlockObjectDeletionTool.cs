using System.Collections.Generic;
using System.Linq;
using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Localization;
using Timberborn.TerrainPhysics;
using Timberborn.TerrainSystemRendering;
using Timberborn.ToolSystem;
using Timberborn.ToolSystemUI;
using Timberborn.UndoSystem;

namespace Timberborn.BlockObjectTools;

public class EntityBlockObjectDeletionTool : BlockObjectDeletionTool<EntityComponent>, IDevModeTool, ITool
{
	private static readonly string ToolDescriptionLocKey = "DeletionTool.Description.Objects";

	private static readonly string ToolTitleLocKey = "DeletionTool.Title.Objects";

	private readonly ILoc _loc;

	private readonly List<IBlockObjectDeletionBlocker> _deletionBlockers = new List<IBlockObjectDeletionBlocker>();

	public bool IsDevMode => true;

	protected override string ToolPromptLocKey => "DeletionTool.Prompt.Objects";

	protected override string CursorKey => "DeleteBuildingCursor";

	public EntityBlockObjectDeletionTool(InputService inputService, AreaBlockObjectAndTerrainPicker areaBlockObjectAndTerrainPicker, EntityService entityService, BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory, CursorService cursorService, ILoc loc, BlockObjectModelBlockadeIgnorer blockObjectModelBlockadeIgnorer, ISpecService specService, ILevelVisibilityService levelVisibilityService, DialogBoxShower dialogBoxShower, TerrainDestroyer terrainDestroyer, TerrainHighlightingService terrainHighlightingService, IUndoRegistry undoRegistry)
		: base(inputService, areaBlockObjectAndTerrainPicker, entityService, blockObjectSelectionDrawerFactory, cursorService, blockObjectModelBlockadeIgnorer, specService, levelVisibilityService, dialogBoxShower, terrainDestroyer, terrainHighlightingService, undoRegistry)
	{
		_loc = loc;
	}

	public override ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder(_loc.T(ToolTitleLocKey)).AddSection(_loc.T(ToolDescriptionLocKey)).Build();
	}

	protected override bool IsBlockObjectValid(BlockObject blockObject)
	{
		return !IsStackedDeletionBlocked(blockObject);
	}

	private bool IsStackedDeletionBlocked(BlockObject blockObject)
	{
		blockObject.GetComponents(_deletionBlockers);
		bool result = _deletionBlockers.Where((IBlockObjectDeletionBlocker blocker) => blocker.NoForcedDelete).Any((IBlockObjectDeletionBlocker blocker) => blocker.IsStackedDeletionBlocked);
		_deletionBlockers.Clear();
		return result;
	}
}
