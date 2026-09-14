using Timberborn.BlueprintSystem;
using Timberborn.Forestry;
using Timberborn.ForestryUI;
using Timberborn.Localization;
using Timberborn.ToolButtonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class MarkTreesTutorialStepDeserializer : IStepDeserializer
{
	private static readonly string MarkTreesLocKey = "Tutorial.MarkTrees";

	private readonly TreeCuttingArea _treeCuttingArea;

	private readonly ILoc _loc;

	private readonly ToolButtonService _toolButtonService;

	public MarkTreesTutorialStepDeserializer(TreeCuttingArea treeCuttingArea, ILoc loc, ToolButtonService toolButtonService)
	{
		_treeCuttingArea = treeCuttingArea;
		_loc = loc;
		_toolButtonService = toolButtonService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is MarkTreesTutorialStepSpec)
		{
			tutorialStep = Create();
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create()
	{
		ToolButton toolButton = _toolButtonService.GetToolButton<TreeCuttingAreaSelectionTool>();
		ToolGroupButton toolGroupButton = _toolButtonService.GetToolGroupButton(toolButton);
		return TutorialStep.Create(new MarkTreesTutorialStep(_treeCuttingArea, _loc.T(MarkTreesLocKey)), toolGroupButton, toolButton);
	}
}
