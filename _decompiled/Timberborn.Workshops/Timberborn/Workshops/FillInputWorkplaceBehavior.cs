using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.Emptying;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.WorkSystem;

namespace Timberborn.Workshops;

public class FillInputWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private Inventories _inventories;

	private Emptiable _emptiable;

	private readonly SortedSet<GoodAmount> _inputGoodsOrdered = new SortedSet<GoodAmount>(new GoodAmountComparer());

	public void Awake()
	{
		_inventories = GetComponent<Inventories>();
		_emptiable = GetComponent<Emptiable>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!_emptiable || !_emptiable.IsMarkedForEmptying)
		{
			CarrierInventoryFinder component = agent.GetComponent<CarrierInventoryFinder>();
			foreach (Inventory enabledInventory in _inventories.EnabledInventories)
			{
				if (StartCarrying(enabledInventory, component))
				{
					return Decision.ReleaseNextTick();
				}
			}
		}
		return Decision.ReleaseNow();
	}

	private bool StartCarrying(Inventory inventory, CarrierInventoryFinder carrierInventoryFinder)
	{
		foreach (string inputGood in inventory.InputGoods)
		{
			_inputGoodsOrdered.Add(new GoodAmount(inputGood, inventory.UnreservedAmountInStock(inputGood)));
		}
		foreach (GoodAmount item in _inputGoodsOrdered)
		{
			if (carrierInventoryFinder.TryCarryFromAnyInventory(item.GoodId, inventory))
			{
				_inputGoodsOrdered.Clear();
				return true;
			}
		}
		_inputGoodsOrdered.Clear();
		return false;
	}
}
