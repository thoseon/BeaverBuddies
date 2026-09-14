using System.Collections.Generic;
using Timberborn.Goods;
using Timberborn.InventoryNeedSystem;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.SimpleOutputBuildings;

internal class SimpleOutputInventoryInitializer : IDedicatedDecoratorInitializer<SimpleOutputInventory, Inventory>
{
	private static readonly string InventoryComponentName = "SimpleOutputInventory";

	private readonly IGoodService _goodService;

	private readonly InventoryNeedBehaviorInitializer _inventoryNeedBehaviorInitializer;

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public SimpleOutputInventoryInitializer(IGoodService goodService, InventoryNeedBehaviorInitializer inventoryNeedBehaviorInitializer, InventoryInitializerFactory inventoryInitializerFactory)
	{
		_goodService = goodService;
		_inventoryNeedBehaviorInitializer = inventoryNeedBehaviorInitializer;
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(SimpleOutputInventory subject, Inventory decorator)
	{
		IAllowedGoodProvider component = subject.GetComponent<IAllowedGoodProvider>();
		SimpleOutputInventorySpec component2 = subject.GetComponent<SimpleOutputInventorySpec>();
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.CreateWithUnlimitedCapacity(decorator, InventoryComponentName);
		inventoryInitializer.HasPublicOutput();
		AddGoodDisallower(subject, inventoryInitializer);
		if (component2.IgnorableCapacity)
		{
			inventoryInitializer.SetIgnorableCapacity();
		}
		AllowGoodsAsTakeable(inventoryInitializer, component2.Capacity, component);
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
		_inventoryNeedBehaviorInitializer.AddNeedBehavior(decorator);
	}

	private static void AddGoodDisallower(SimpleOutputInventory subject, InventoryInitializer inventoryInitializer)
	{
		IGoodDisallower component = subject.GetComponent<IGoodDisallower>();
		inventoryInitializer.AddGoodDisallower(component);
	}

	private void AllowGoodsAsTakeable(InventoryInitializer inventoryInitializer, int allowedAmount, IAllowedGoodProvider provider)
	{
		foreach (string item in (IEnumerable<string>)(provider?.GetAllowedGoods() ?? ((object)_goodService.Goods)))
		{
			StorableGood storableGood = StorableGood.CreateAsTakeable(item);
			StorableGoodAmount good = new StorableGoodAmount(storableGood, allowedAmount);
			inventoryInitializer.AddAllowedGood(good);
		}
	}
}
