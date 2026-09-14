using System.Collections.Generic;
using Timberborn.BottomBarSystem;
using Timberborn.PrioritySystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.BuilderPrioritySystemUI;

internal class BuilderPrioritiesButton : IBottomBarElementsProvider
{
	private static readonly string ToolGroupId = "BuilderPriority";

	private readonly BuilderPrioritiesButtonFactory _builderPrioritiesButtonFactory;

	private readonly ToolGroupButtonFactory _toolGroupButtonFactory;

	private readonly ToolGroupService _toolGroupService;

	public BuilderPrioritiesButton(BuilderPrioritiesButtonFactory builderPrioritiesButtonFactory, ToolGroupButtonFactory toolGroupButtonFactory, ToolGroupService toolGroupService)
	{
		_builderPrioritiesButtonFactory = builderPrioritiesButtonFactory;
		_toolGroupButtonFactory = toolGroupButtonFactory;
		_toolGroupService = toolGroupService;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolGroupSpec toolGroup = _toolGroupService.GetGroup(ToolGroupId);
		ToolGroupButton toolGroupButton = _toolGroupButtonFactory.CreateBlue(toolGroup);
		foreach (Priority item in Priorities.Ascending)
		{
			ToolButton toolButton = _builderPrioritiesButtonFactory.CreateButton(item, toolGroupButton.ToolButtonsElement);
			_toolGroupService.AssignToGroup(toolGroup, toolButton.Tool);
			toolGroupButton.AddTool(toolButton);
		}
		yield return BottomBarElement.CreateMultiLevel(toolGroupButton.Root, toolGroupButton.ToolButtonsElement);
	}
}
