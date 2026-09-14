using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.WorkSystem;

namespace Timberborn.Reproduction;

internal class BringNutrientBehavior : CommunityServiceBehavior, IAwakableComponent
{
	private Citizen _citizen;

	private CarrierInventoryFinder _carrierInventoryFinder;

	public void Awake()
	{
		_citizen = GetComponent<Citizen>();
		_carrierInventoryFinder = GetComponent<CarrierInventoryFinder>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		DistrictCenter assignedDistrict = _citizen.AssignedDistrict;
		if ((bool)assignedDistrict)
		{
			DistrictBreedingPodService component = assignedDistrict.GetComponent<DistrictBreedingPodService>();
			BreedingPod breedingPod;
			while (component.TryDequeueNeedingNutrients(out breedingPod))
			{
				if ((bool)breedingPod && StartCarrying(breedingPod))
				{
					return Decision.ReleaseNextTick();
				}
			}
		}
		return Decision.ReleaseNow();
	}

	private bool StartCarrying(BreedingPod breedingPod)
	{
		foreach (GoodAmountSpec item in breedingPod.NutrientsPerCycle)
		{
			if (_carrierInventoryFinder.TryCarryFromAnyInventory(item.Id, breedingPod.Inventory))
			{
				return true;
			}
		}
		return false;
	}
}
