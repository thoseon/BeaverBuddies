using System.Collections.Generic;
using System.Linq;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;
using UnityEngine;

namespace Timberborn.GoodConsumingBuildingSystem;

internal class GoodConsumingBuildingInventoryInitializer : IDedicatedDecoratorInitializer<GoodConsumingBuilding, Inventory>
{
	private static readonly string InventoryComponentName = "GoodConsumingBuilding";

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public GoodConsumingBuildingInventoryInitializer(InventoryInitializerFactory inventoryInitializerFactory)
	{
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(GoodConsumingBuilding subject, Inventory decorator)
	{
		GoodConsumingBuildingSpec component = subject.GetComponent<GoodConsumingBuildingSpec>();
		List<StorableGoodAmount> list = new List<StorableGoodAmount>();
		foreach (ConsumedGoodSpec consumedGood in component.ConsumedGoods)
		{
			StorableGood storableGood = StorableGood.CreateAsGivable(consumedGood.GoodId);
			int amount = Mathf.CeilToInt(consumedGood.GoodPerHour * (float)component.FullInventoryWorkHours);
			list.Add(new StorableGoodAmount(storableGood, amount));
		}
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.Create(decorator, list.Sum((StorableGoodAmount good) => good.Amount), InventoryComponentName);
		inventoryInitializer.AddAllowedGoods(list);
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
	}
}
