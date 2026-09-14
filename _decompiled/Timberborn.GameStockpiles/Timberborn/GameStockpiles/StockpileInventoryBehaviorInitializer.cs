using Timberborn.InventoryNeedSystem;
using Timberborn.InventorySystem;
using Timberborn.SingletonSystem;
using Timberborn.Stockpiles;

namespace Timberborn.GameStockpiles;

internal class StockpileInventoryBehaviorInitializer : ILoadableSingleton
{
	private readonly InventoryNeedBehaviorInitializer _inventoryNeedBehaviorInitializer;

	private readonly StockpileInventoryInitializer _stockpileInventoryInitializer;

	public StockpileInventoryBehaviorInitializer(InventoryNeedBehaviorInitializer inventoryNeedBehaviorInitializer, StockpileInventoryInitializer stockpileInventoryInitializer)
	{
		_inventoryNeedBehaviorInitializer = inventoryNeedBehaviorInitializer;
		_stockpileInventoryInitializer = stockpileInventoryInitializer;
	}

	public void Load()
	{
		_stockpileInventoryInitializer.InventoryInitialized += OnInventoryInitialized;
	}

	private void OnInventoryInitialized(object sender, Inventory inventory)
	{
		_inventoryNeedBehaviorInitializer.AddNeedBehavior(inventory);
	}
}
