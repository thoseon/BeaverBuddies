using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;
using Timberborn.WorkSystem;

namespace Timberborn.StockpilePrioritySystem;

public class SupplyGoodWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private GoodSupplier _goodSupplier;

	private SingleGoodAllower _singleGoodAllower;

	private Inventory _inventory;

	public void Awake()
	{
		_goodSupplier = GetComponent<GoodSupplier>();
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		_inventory = GetComponent<Stockpile>().Inventory;
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (CanGiveGoods() && StartCarrying(agent))
		{
			return Decision.ReleaseNextTick();
		}
		return Decision.ReleaseNow();
	}

	private bool CanGiveGoods()
	{
		if (_goodSupplier.IsSupplying && _inventory.Enabled && _singleGoodAllower.HasAllowedGood)
		{
			return _singleGoodAllower.AllowedAmount(_singleGoodAllower.AllowedGood) > 0;
		}
		return false;
	}

	private bool StartCarrying(BehaviorAgent agent)
	{
		CarrierInventoryFinder component = agent.GetComponent<CarrierInventoryFinder>();
		string allowedGood = _singleGoodAllower.AllowedGood;
		return component.TryCarryToAnyInventory(allowedGood, _inventory, CanGiveTo);
	}

	private static bool CanGiveTo(Inventory inventory)
	{
		GoodSupplier component = inventory.GetComponent<GoodSupplier>();
		if ((bool)component)
		{
			return !component.IsSupplying;
		}
		return true;
	}
}
