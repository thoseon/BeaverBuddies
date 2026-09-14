using System.Collections.Generic;
using Timberborn.BlockObjectTools;
using Timberborn.BottomBarSystem;
using Timberborn.DeconstructionSystemUI;
using Timberborn.DemolishingUI;
using Timberborn.RecoveredGoodSystemUI;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.DemolishingToolGroupSystem;

internal class DemolishingButton : IBottomBarElementsProvider
{
	private static readonly string ToolGroupId = "Demolishing";

	private static readonly string DeleteRecoveredGoodStackToolImageKey = "DeleteRecoveredGoodStackToolIcon";

	private static readonly string DeleteBuildingImageKey = "DeleteObjectIcon";

	private static readonly string DemolishToolImageKey = "DemolishResourcesTool";

	private static readonly string CancelToolImageKey = "CancelToolIcon";

	private readonly RecoveredGoodStackDeletionTool _recoveredGoodStackDeletionTool;

	private readonly DemolishableSelectionTool _demolishableSelectionTool;

	private readonly DemolishableUnselectionTool _demolishableUnselectionTool;

	private readonly BuildingDeconstructionTool _buildingDeconstructionTool;

	private readonly EntityBlockObjectDeletionTool _entityBlockObjectDeletionTool;

	private readonly ToolButtonFactory _toolButtonFactory;

	private readonly ToolGroupButtonFactory _toolGroupButtonFactory;

	private readonly ToolGroupService _toolGroupService;

	public DemolishingButton(DemolishableSelectionTool demolishableSelectionTool, DemolishableUnselectionTool demolishableUnselectionTool, ToolButtonFactory toolButtonFactory, ToolGroupButtonFactory toolGroupButtonFactory, BuildingDeconstructionTool buildingDeconstructionTool, EntityBlockObjectDeletionTool entityBlockObjectDeletionTool, RecoveredGoodStackDeletionTool recoveredGoodStackDeletionTool, ToolGroupService toolGroupService)
	{
		_demolishableSelectionTool = demolishableSelectionTool;
		_demolishableUnselectionTool = demolishableUnselectionTool;
		_toolButtonFactory = toolButtonFactory;
		_toolGroupButtonFactory = toolGroupButtonFactory;
		_buildingDeconstructionTool = buildingDeconstructionTool;
		_entityBlockObjectDeletionTool = entityBlockObjectDeletionTool;
		_recoveredGoodStackDeletionTool = recoveredGoodStackDeletionTool;
		_toolGroupService = toolGroupService;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolGroupSpec toolGroup = _toolGroupService.GetGroup(ToolGroupId);
		ToolGroupButton toolGroupButton = _toolGroupButtonFactory.CreateBlue(toolGroup);
		AddTool(_buildingDeconstructionTool, DeleteBuildingImageKey, toolGroup, toolGroupButton);
		AddTool(_recoveredGoodStackDeletionTool, DeleteRecoveredGoodStackToolImageKey, toolGroup, toolGroupButton);
		AddTool(_demolishableSelectionTool, DemolishToolImageKey, toolGroup, toolGroupButton);
		AddTool(_entityBlockObjectDeletionTool, DeleteBuildingImageKey, toolGroup, toolGroupButton);
		AddTool(_demolishableUnselectionTool, CancelToolImageKey, toolGroup, toolGroupButton);
		yield return BottomBarElement.CreateMultiLevel(toolGroupButton.Root, toolGroupButton.ToolButtonsElement);
	}

	private void AddTool(ITool tool, string imageName, ToolGroupSpec toolGroup, ToolGroupButton toolGroupButton)
	{
		ToolButton button = _toolButtonFactory.Create(tool, imageName, toolGroupButton.ToolButtonsElement);
		toolGroupButton.AddTool(button);
		_toolGroupService.AssignToGroup(toolGroup, tool);
	}
}
