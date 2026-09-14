using System.Collections.Generic;
using System.Linq;
using Timberborn.Debugging;
using Timberborn.EntitySystem;
using Timberborn.GoodConsumingBuildingSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Reproduction;
using Timberborn.Workshops;

namespace Timberborn.InventorySystemUI;

internal class InventoryFillerDevModule : IDevModule
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	public InventoryFillerDevModule(EntityComponentRegistry entityComponentRegistry)
	{
		_entityComponentRegistry = entityComponentRegistry;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Fill input inventories", FillInventories)).Build();
	}

	private void FillInventories()
	{
		FillInventories(from manufactory in _entityComponentRegistry.GetEnabled<Manufactory>()
			select manufactory.Inventory);
		FillInventories(from manufactory in _entityComponentRegistry.GetEnabled<GoodConsumingBuilding>()
			select manufactory.Inventory);
		FillInventories(from manufactory in _entityComponentRegistry.GetEnabled<BreedingPod>()
			select manufactory.Inventory);
	}

	private void FillInventories(IEnumerable<Inventory> inventories)
	{
		foreach (Inventory inventory in inventories)
		{
			FillInventory(inventory);
		}
	}

	private void FillInventory(Inventory inventory)
	{
		foreach (string inputGood in inventory.InputGoods)
		{
			GiveGood(inputGood, inventory);
		}
	}

	private static void GiveGood(string goodId, Inventory inventory)
	{
		int num = inventory.UnreservedCapacity(goodId);
		if (num > 0)
		{
			inventory.GiveExisting(new GoodAmount(goodId, num));
		}
	}
}
