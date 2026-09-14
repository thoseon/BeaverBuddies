using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class DecreasePriorityStepDeserializer : IStepDeserializer
{
	private readonly ILoc _loc;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly BuildingService _buildingService;

	public DecreasePriorityStepDeserializer(ILoc loc, EntitySelectionService entitySelectionService, BuildingService buildingService)
	{
		_loc = loc;
		_entitySelectionService = entitySelectionService;
		_buildingService = buildingService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is DecreasePriorityStepSpec decreasePriorityStepSpec)
		{
			tutorialStep = Create(decreasePriorityStepSpec.TemplateName);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName)
	{
		LabeledEntitySpec spec = _buildingService.GetBuildingTemplate(templateName).GetSpec<LabeledEntitySpec>();
		string description = _loc.T("Tutorial.DecreaseWorkplacePriority", _loc.T(spec.DisplayNameLocKey));
		return TutorialStep.Create(new DecreasePriorityStep(_entitySelectionService, description, templateName));
	}
}
