using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;
using Timberborn.WorkSystem;

namespace Timberborn.StockpilePrioritySystem;

internal class ObtainGoodWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private Inventory _inventory;

	private GoodObtainer _goodObtainer;

	private SingleGoodAllower _singleGoodAllower;

	public void Awake()
	{
		_inventory = GetComponent<Stockpile>().Inventory;
		_goodObtainer = GetComponent<GoodObtainer>();
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (CanObtain() && StartCarrying(agent))
		{
			return Decision.ReleaseNextTick();
		}
		return Decision.ReleaseNow();
	}

	private bool CanObtain()
	{
		if (_goodObtainer.IsObtaining && _inventory.Enabled && _singleGoodAllower.HasAllowedGood)
		{
			return _singleGoodAllower.AllowedAmount(_singleGoodAllower.AllowedGood) > 0;
		}
		return false;
	}

	private bool StartCarrying(BehaviorAgent agent)
	{
		CarrierInventoryFinder component = agent.GetComponent<CarrierInventoryFinder>();
		string allowedGood = _singleGoodAllower.AllowedGood;
		return component.TryCarryFromAnyInventory(allowedGood, _inventory, CanObtainFrom);
	}

	private static bool CanObtainFrom(Inventory inventory)
	{
		GoodObtainer component = inventory.GetComponent<GoodObtainer>();
		if ((bool)component)
		{
			return !component.IsObtaining;
		}
		return true;
	}
}
