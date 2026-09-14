using Timberborn.BlockObjectTools;
using Timberborn.BlueprintSystem;
using Timberborn.BuildingTools;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class UnlockBuildingTutorialStepDeserializer : IStepDeserializer
{
	private readonly BuildingUnlockingService _buildingUnlockingService;

	private readonly BuildingService _buildingService;

	private readonly ILoc _loc;

	private readonly ToolButtonService _toolButtonService;

	private readonly UnlockSectionController _unlockSectionController;

	public UnlockBuildingTutorialStepDeserializer(BuildingUnlockingService buildingUnlockingService, BuildingService buildingService, ILoc loc, ToolButtonService toolButtonService, UnlockSectionController unlockSectionController)
	{
		_buildingUnlockingService = buildingUnlockingService;
		_buildingService = buildingService;
		_loc = loc;
		_toolButtonService = toolButtonService;
		_unlockSectionController = unlockSectionController;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is UnlockBuildingTutorialStepSpec unlockBuildingTutorialStepSpec)
		{
			tutorialStep = Create(unlockBuildingTutorialStepSpec.TemplateName);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName)
	{
		BuildingSpec buildingTemplate = _buildingService.GetBuildingTemplate(templateName);
		LabeledEntitySpec spec = buildingTemplate.GetSpec<LabeledEntitySpec>();
		string localizedBuildingName = _loc.T(spec.DisplayNameLocKey);
		ToolButton toolButton = _toolButtonService.GetToolButton((BlockObjectTool tool) => tool.Template.GetSpec<TemplateSpec>().IsNamedExactly(templateName));
		ToolGroupButton toolGroupButton = _toolButtonService.GetToolGroupButton(toolButton);
		return TutorialStep.Create(new UnlockBuildingTutorialStep(_buildingUnlockingService, _loc, buildingTemplate.GetSpec<BuildingSpec>(), localizedBuildingName), toolGroupButton, toolButton, delegate(bool state)
		{
			Highlight(state, toolButton.Tool);
		});
	}

	private void Highlight(bool state, ITool tool)
	{
		_unlockSectionController.ToggleHighlight(state, tool);
	}
}
