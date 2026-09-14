using Timberborn.Buildings;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class AccumulateScienceForBuildingStep : ITutorialStep
{
	private static readonly string AccumulateScienceLocKey = "Tutorial.Science.AccumulateScience";

	private readonly ScienceService _scienceService;

	private readonly BuildingUnlockingService _buildingUnlockingService;

	private readonly ILoc _loc;

	private readonly BuildingSpec _buildingSpec;

	private readonly int _requiredPoints;

	public AccumulateScienceForBuildingStep(ScienceService scienceService, BuildingUnlockingService buildingUnlockingService, ILoc loc, BuildingSpec buildingSpec)
	{
		_scienceService = scienceService;
		_buildingUnlockingService = buildingUnlockingService;
		_loc = loc;
		_buildingSpec = buildingSpec;
		_requiredPoints = _buildingSpec.ScienceCost;
	}

	public string Description()
	{
		int param = ((_buildingUnlockingService.Unlocked(_buildingSpec) || _scienceService.SciencePoints > _requiredPoints) ? _requiredPoints : _scienceService.SciencePoints);
		return _loc.T(AccumulateScienceLocKey, param, _requiredPoints);
	}

	public bool Achieved()
	{
		if (_scienceService.SciencePoints < _requiredPoints)
		{
			return _buildingUnlockingService.Unlocked(_buildingSpec);
		}
		return true;
	}
}
