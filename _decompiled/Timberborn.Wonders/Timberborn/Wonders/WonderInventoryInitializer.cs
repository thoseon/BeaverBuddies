using System.Collections.Generic;
using System.Linq;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Wonders;

internal class WonderInventoryInitializer : IDedicatedDecoratorInitializer<WonderInventory, Inventory>
{
	private static readonly string InventoryComponentName = "Wonder";

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	private readonly Inventory _inventory;

	public WonderInventoryInitializer(InventoryInitializerFactory inventoryInitializerFactory)
	{
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(WonderInventory subject, Inventory decorator)
	{
		List<StorableGoodAmount> list = subject.GetComponent<WonderInventorySpec>().RequiredGoods.Select((GoodAmountSpec good) => new StorableGoodAmount(StorableGood.CreateAsGivable(good.Id), good.Amount)).ToList();
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.Create(decorator, list.Sum((StorableGoodAmount g) => g.Amount), InventoryComponentName);
		inventoryInitializer.AddAllowedGoods(list);
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
	}
}
