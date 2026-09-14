using System.Collections.Generic;
using Timberborn.BottomBarSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.ForestryUI;

internal class TreeCuttingAreaButton : IBottomBarElementsProvider
{
	private static readonly string ToolGroupId = "TreeCutting";

	private static readonly string SelectionToolImageKey = "TreeCuttingAreaSelectionTool";

	private static readonly string UnselectionToolImageKey = "CancelToolIcon";

	private readonly TreeCuttingAreaSelectionTool _treeCuttingAreaSelectionTool;

	private readonly TreeCuttingAreaUnselectionTool _treeCuttingAreaUnselectionTool;

	private readonly ToolButtonFactory _toolButtonFactory;

	private readonly ToolGroupButtonFactory _toolGroupButtonFactory;

	private readonly ToolGroupService _toolGroupService;

	public TreeCuttingAreaButton(TreeCuttingAreaSelectionTool treeCuttingAreaSelectionTool, TreeCuttingAreaUnselectionTool treeCuttingAreaUnselectionTool, ToolButtonFactory toolButtonFactory, ToolGroupButtonFactory toolGroupButtonFactory, ToolGroupService toolGroupService)
	{
		_treeCuttingAreaSelectionTool = treeCuttingAreaSelectionTool;
		_treeCuttingAreaUnselectionTool = treeCuttingAreaUnselectionTool;
		_toolButtonFactory = toolButtonFactory;
		_toolGroupButtonFactory = toolGroupButtonFactory;
		_toolGroupService = toolGroupService;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolGroupSpec toolGroup = _toolGroupService.GetGroup(ToolGroupId);
		ToolGroupButton toolGroupButton = _toolGroupButtonFactory.CreateBlue(toolGroup);
		AddTool(_treeCuttingAreaSelectionTool, SelectionToolImageKey, toolGroup, toolGroupButton);
		AddTool(_treeCuttingAreaUnselectionTool, UnselectionToolImageKey, toolGroup, toolGroupButton);
		yield return BottomBarElement.CreateMultiLevel(toolGroupButton.Root, toolGroupButton.ToolButtonsElement);
	}

	private void AddTool(ITool tool, string imageName, ToolGroupSpec toolGroup, ToolGroupButton toolGroupButton)
	{
		ToolButton button = _toolButtonFactory.Create(tool, imageName, toolGroupButton.ToolButtonsElement);
		toolGroupButton.AddTool(button);
		_toolGroupService.AssignToGroup(toolGroup, tool);
	}
}
