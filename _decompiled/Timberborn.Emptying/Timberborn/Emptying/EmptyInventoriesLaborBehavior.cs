using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.GameDistricts;
using Timberborn.InventorySystem;
using Timberborn.LaborSystem;

namespace Timberborn.Emptying;

public class EmptyInventoriesLaborBehavior : LaborBehavior, IAwakableComponent
{
	private DistrictBuilding _districtBuilding;

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		DistrictCenter district = _districtBuilding.District;
		if ((bool)district)
		{
			DistrictEmptiableInventoriesRegistry component = district.GetComponent<DistrictEmptiableInventoriesRegistry>();
			EmptyingStarter component2 = agent.GetComponent<EmptyingStarter>();
			foreach (Inventories emptiableInventory in component.EmptiableInventories)
			{
				foreach (Inventory enabledInventory in emptiableInventory.EnabledInventories)
				{
					if ((bool)enabledInventory && component2.StartEmptying(enabledInventory))
					{
						return Decision.ReleaseNextTick();
					}
				}
			}
		}
		return Decision.ReleaseNow();
	}
}
