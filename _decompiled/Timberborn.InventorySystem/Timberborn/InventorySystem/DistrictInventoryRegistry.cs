using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Goods;

namespace Timberborn.InventorySystem;

public class DistrictInventoryRegistry : BaseComponent, IAwakableComponent
{
	private readonly IGoodService _goodService;

	private InventoryRegistry _publicInventoryRegistry;

	public ReadOnlyHashSet<Inventory> Inventories => _publicInventoryRegistry.Inventories;

	public event EventHandler<Inventory> InventoryRegistered;

	public event EventHandler<Inventory> InventoryUnregistered;

	public DistrictInventoryRegistry(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public void Awake()
	{
		_publicInventoryRegistry = new InventoryRegistry(_goodService.Goods);
	}

	public void Add(Inventory inventory)
	{
		if (inventory.PublicInput || inventory.PublicOutput)
		{
			_publicInventoryRegistry.RegisterInventory(inventory);
		}
		InventoryRegistered?.Invoke(this, inventory);
	}

	public void Remove(Inventory inventory)
	{
		if (inventory.PublicInput || inventory.PublicOutput)
		{
			_publicInventoryRegistry.UnregisterInventory(inventory);
		}
		InventoryUnregistered?.Invoke(this, inventory);
	}

	public ReadOnlyHashSet<Inventory> ActiveInventoriesWithStock(string goodId)
	{
		return _publicInventoryRegistry.InventoriesWithStock(goodId);
	}

	public ReadOnlyHashSet<Inventory> ActiveInventoriesWithCapacity(string goodId)
	{
		return _publicInventoryRegistry.InventoriesWithCapacity(goodId);
	}
}
