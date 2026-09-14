using Timberborn.BlueprintSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.Planting;
using Timberborn.PlantingUI;
using Timberborn.TemplateSystem;
using Timberborn.ToolButtonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class MarkPlantablesTutorialStepDeserializer : IStepDeserializer
{
	private readonly PlantableResourceCounter _plantableResourceCounter;

	private readonly ILoc _loc;

	private readonly ToolButtonService _toolButtonService;

	public MarkPlantablesTutorialStepDeserializer(PlantableResourceCounter plantableResourceCounter, ILoc loc, ToolButtonService toolButtonService)
	{
		_plantableResourceCounter = plantableResourceCounter;
		_loc = loc;
		_toolButtonService = toolButtonService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is MarkPlantablesTutorialStepSpec markPlantablesTutorialStepSpec)
		{
			tutorialStep = Create(markPlantablesTutorialStepSpec.TemplateName, markPlantablesTutorialStepSpec.RequiredAmount);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName, int requiredAmount)
	{
		ToolButton toolButton = _toolButtonService.GetToolButton((PlantingTool plantingTool) => plantingTool.PlantableSpec.GetSpec<TemplateSpec>().IsNamedExactly(templateName));
		PlantableSpec plantableSpec = ((PlantingTool)toolButton.Tool).PlantableSpec;
		string localizedResourceName = _loc.T(plantableSpec.GetSpec<LabeledEntitySpec>().DisplayNameLocKey);
		ToolGroupButton toolGroupButton = _toolButtonService.GetToolGroupButton(toolButton);
		return TutorialStep.Create(new MarkPlantablesTutorialStep(_plantableResourceCounter, _loc, templateName, requiredAmount, localizedResourceName), toolGroupButton, toolButton);
	}
}
