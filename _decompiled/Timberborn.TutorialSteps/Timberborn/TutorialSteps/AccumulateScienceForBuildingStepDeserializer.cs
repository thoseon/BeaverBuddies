using Timberborn.BlueprintSystem;
using Timberborn.Buildings;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class AccumulateScienceForBuildingStepDeserializer : IStepDeserializer
{
	private readonly BuildingUnlockingService _buildingUnlockingService;

	private readonly BuildingService _buildingService;

	private readonly ScienceService _scienceService;

	private readonly ILoc _loc;

	public AccumulateScienceForBuildingStepDeserializer(BuildingUnlockingService buildingUnlockingService, BuildingService buildingService, ScienceService scienceService, ILoc loc)
	{
		_buildingUnlockingService = buildingUnlockingService;
		_buildingService = buildingService;
		_scienceService = scienceService;
		_loc = loc;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is AccumulateScienceForBuildingStepSpec accumulateScienceForBuildingStepSpec)
		{
			tutorialStep = Create(accumulateScienceForBuildingStepSpec.TemplateName);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(string templateName)
	{
		BuildingSpec buildingTemplate = _buildingService.GetBuildingTemplate(templateName);
		return TutorialStep.Create(new AccumulateScienceForBuildingStep(_scienceService, _buildingUnlockingService, _loc, buildingTemplate));
	}
}
