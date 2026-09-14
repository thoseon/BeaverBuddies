using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.GameDistricts;
using Timberborn.InventorySystem;
using Timberborn.LaborSystem;

namespace Timberborn.Emptying;

public class RemoveUnwantedStockLaborBehavior : LaborBehavior, IAwakableComponent
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
			DistrictUnwantedStockInventoryRegistry component = district.GetComponent<DistrictUnwantedStockInventoryRegistry>();
			EmptyingStarter component2 = agent.GetComponent<EmptyingStarter>();
			foreach (Inventory item in component.InventoriesWithUnwantedStock)
			{
				if ((bool)item && component2.StartEmptyingUnwantedStock(item))
				{
					return Decision.ReleaseNextTick();
				}
			}
		}
		return Decision.ReleaseNow();
	}
}
