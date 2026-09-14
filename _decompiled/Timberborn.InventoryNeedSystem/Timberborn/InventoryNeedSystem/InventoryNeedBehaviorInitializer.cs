using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.InventoryNeedSystem;

public class InventoryNeedBehaviorInitializer
{
	private readonly IGoodService _goodService;

	public InventoryNeedBehaviorInitializer(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public void AddNeedBehavior(Inventory inventory)
	{
		if (!inventory.GetComponent<InventoryGoodConsumptionBlocker>() && AllowsTakeableConsumableGood(inventory))
		{
			inventory.GetComponent<InventoryNeedBehavior>().Initialize(inventory);
		}
	}

	private bool AllowsTakeableConsumableGood(Inventory inventory)
	{
		foreach (StorableGoodAmount allowedGood in inventory.AllowedGoods)
		{
			if (allowedGood.StorableGood.Takeable && _goodService.GetGood(allowedGood.StorableGood.GoodId).HasConsumptionEffects)
			{
				return true;
			}
		}
		return false;
	}
}
