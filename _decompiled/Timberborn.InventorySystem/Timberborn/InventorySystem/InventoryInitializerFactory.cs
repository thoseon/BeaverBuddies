using Timberborn.Goods;

namespace Timberborn.InventorySystem;

public class InventoryInitializerFactory
{
	private readonly IGoodService _goodService;

	public InventoryInitializerFactory(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public InventoryInitializer Create(Inventory inventory, int capacity, string componentName)
	{
		return new InventoryInitializer(_goodService, inventory, capacity, componentName);
	}

	public InventoryInitializer CreateWithUnlimitedCapacity(Inventory inventory, string componentName)
	{
		return Create(inventory, int.MaxValue, componentName);
	}
}
