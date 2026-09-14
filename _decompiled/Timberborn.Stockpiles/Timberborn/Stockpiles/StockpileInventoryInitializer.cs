using System;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Stockpiles;

public class StockpileInventoryInitializer : IDedicatedDecoratorInitializer<Stockpile, Inventory>
{
	private static readonly string InventoryComponentName = "Stockpile";

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public event EventHandler<Inventory> InventoryInitialized;

	public StockpileInventoryInitializer(InventoryInitializerFactory inventoryInitializerFactory)
	{
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(Stockpile subject, Inventory decorator)
	{
		StockpileSpec component = subject.GetComponent<StockpileSpec>();
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.Create(decorator, component.MaxCapacity, InventoryComponentName);
		inventoryInitializer.AddAllowedGoodType(component.WhitelistedGoodType);
		inventoryInitializer.HasPublicOutput();
		inventoryInitializer.HasPublicInput();
		SingleGoodAllower component2 = subject.GetComponent<SingleGoodAllower>();
		component2.Initialize(decorator);
		inventoryInitializer.AddGoodDisallower(component2);
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
		InventoryInitialized?.Invoke(this, decorator);
	}
}
