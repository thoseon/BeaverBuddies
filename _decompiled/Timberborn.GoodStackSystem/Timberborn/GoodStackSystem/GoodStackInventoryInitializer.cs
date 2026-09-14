using System.Collections.Generic;
using System.Linq;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GoodStackSystem;

internal class GoodStackInventoryInitializer : IDedicatedDecoratorInitializer<IGoodStackInventory, Inventory>
{
	private static readonly string InventoryComponentName = "GoodStack";

	private readonly IGoodService _goodService;

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public GoodStackInventoryInitializer(IGoodService goodService, InventoryInitializerFactory inventoryInitializerFactory)
	{
		_goodService = goodService;
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(IGoodStackInventory subject, Inventory decorator)
	{
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.CreateWithUnlimitedCapacity(decorator, InventoryComponentName);
		IEnumerable<StorableGoodAmount> goods = from storableGood in _goodService.Goods.Select(StorableGood.CreateAsTakeable)
			select new StorableGoodAmount(storableGood, int.MaxValue);
		inventoryInitializer.AddAllowedGoods(goods);
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
	}
}
