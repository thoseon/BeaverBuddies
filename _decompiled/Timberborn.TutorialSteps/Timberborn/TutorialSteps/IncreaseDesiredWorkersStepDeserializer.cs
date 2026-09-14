using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class IncreaseDesiredWorkersStepDeserializer : IStepDeserializer
{
	private readonly ILoc _loc;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly BuildingService _buildingService;

	public IncreaseDesiredWorkersStepDeserializer(ILoc loc, EntitySelectionService entitySelectionService, BuildingService buildingService)
	{
		_loc = loc;
		_entitySelectionService = entitySelectionService;
		_buildingService = buildingService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is IncreaseDesiredWorkersStepSpec increaseDesiredWorkersStepSpec)
		{
			tutorialStep = Create(increaseDesiredWorkersStepSpec.TemplateName);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName)
	{
		LabeledEntitySpec spec = _buildingService.GetBuildingTemplate(templateName).GetSpec<LabeledEntitySpec>();
		string description = _loc.T("Tutorial.IncreaseDesiredWorkers", _loc.T(spec.DisplayNameLocKey));
		return TutorialStep.Create(new IncreaseDesiredWorkersStep(_entitySelectionService, description, templateName));
	}
}
