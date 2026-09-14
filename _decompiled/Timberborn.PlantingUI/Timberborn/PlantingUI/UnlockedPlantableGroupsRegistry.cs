using System.Collections.Generic;
using Timberborn.Buildings;
using Timberborn.Planting;
using Timberborn.ScienceSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.PlantingUI;

internal class UnlockedPlantableGroupsRegistry : IPostLoadableSingleton
{
	private readonly BuildingService _buildingService;

	private readonly BuildingUnlockingService _buildingUnlockingService;

	private readonly HashSet<string> _unlockedResourceGroups = new HashSet<string>();

	public UnlockedPlantableGroupsRegistry(BuildingService buildingService, BuildingUnlockingService buildingUnlockingService)
	{
		_buildingService = buildingService;
		_buildingUnlockingService = buildingUnlockingService;
	}

	public void PostLoad()
	{
		foreach (BuildingSpec building in _buildingService.Buildings)
		{
			if (_buildingUnlockingService.Unlocked(building))
			{
				AddUnlockedPlantableGroups(building);
			}
		}
	}

	public bool IsLocked(PlantableSpec plantableSpec)
	{
		return !_unlockedResourceGroups.Contains(plantableSpec.ResourceGroup);
	}

	public void AddUnlockedPlantableGroups(BuildingSpec buildingSpec)
	{
		PlanterBuildingSpec spec = buildingSpec.GetSpec<PlanterBuildingSpec>();
		if ((object)spec != null)
		{
			_unlockedResourceGroups.Add(spec.PlantableResourceGroup);
		}
	}
}
