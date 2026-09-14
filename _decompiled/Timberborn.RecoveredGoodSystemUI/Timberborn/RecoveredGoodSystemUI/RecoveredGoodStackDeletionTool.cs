using Timberborn.AreaSelectionSystem;
using Timberborn.AreaSelectionSystemUI;
using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockObjectTools;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Localization;
using Timberborn.RecoveredGoodSystem;
using Timberborn.TerrainPhysics;
using Timberborn.TerrainSystemRendering;
using Timberborn.ToolSystemUI;
using Timberborn.UndoSystem;

namespace Timberborn.RecoveredGoodSystemUI;

public class RecoveredGoodStackDeletionTool : BlockObjectDeletionTool<RecoveredGoodStack>
{
	private static readonly string ToolDescriptionLocKey = "DeletionTool.Description.RecoveredGoodStack";

	private static readonly string ToolTitleLocKey = "DeletionTool.Title.RecoveredGoodStack";

	private readonly ILoc _loc;

	protected override string CursorKey => "DemolishResourcesCursor";

	protected override string ToolPromptLocKey => "RecoveredGoodStack.DeletePrompt";

	public RecoveredGoodStackDeletionTool(InputService inputService, AreaBlockObjectAndTerrainPicker areaBlockObjectAndTerrainPicker, EntityService entityService, BlockObjectSelectionDrawerFactory blockObjectSelectionDrawerFactory, CursorService cursorService, ILoc loc, BlockObjectModelBlockadeIgnorer blockObjectModelBlockadeIgnorer, ISpecService specService, ILevelVisibilityService levelVisibilityService, DialogBoxShower dialogBoxShower, TerrainDestroyer terrainDestroyer, TerrainHighlightingService terrainHighlightingService, IUndoRegistry undoRegistry)
		: base(inputService, areaBlockObjectAndTerrainPicker, entityService, blockObjectSelectionDrawerFactory, cursorService, blockObjectModelBlockadeIgnorer, specService, levelVisibilityService, dialogBoxShower, terrainDestroyer, terrainHighlightingService, undoRegistry)
	{
		_loc = loc;
	}

	public override ToolDescription DescribeTool()
	{
		return new ToolDescription.Builder(_loc.T(ToolTitleLocKey)).AddSection(_loc.T(ToolDescriptionLocKey)).Build();
	}
}
