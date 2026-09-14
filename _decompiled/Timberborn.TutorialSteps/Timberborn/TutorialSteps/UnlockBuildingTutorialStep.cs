using Timberborn.Buildings;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class UnlockBuildingTutorialStep : ITutorialStep
{
	private static readonly string UnlockLocKey = "Tutorial.Science.Unlock";

	private readonly BuildingUnlockingService _buildingUnlockingService;

	private readonly ILoc _loc;

	private readonly BuildingSpec _buildingSpec;

	private readonly string _localizedBuildingName;

	public UnlockBuildingTutorialStep(BuildingUnlockingService buildingUnlockingService, ILoc loc, BuildingSpec buildingSpec, string localizedBuildingName)
	{
		_buildingUnlockingService = buildingUnlockingService;
		_loc = loc;
		_buildingSpec = buildingSpec;
		_localizedBuildingName = localizedBuildingName;
	}

	public string Description()
	{
		return _loc.T(UnlockLocKey, _localizedBuildingName);
	}

	public bool Achieved()
	{
		return _buildingUnlockingService.Unlocked(_buildingSpec);
	}
}
