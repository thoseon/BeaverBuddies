using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Buildings;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ConstructionSites;

internal class ConstructionSiteInventoryInitializer : IDedicatedDecoratorInitializer<ConstructionSite, Inventory>
{
	private static readonly string InventoryComponentName = "ConstructionSite";

	private readonly IGoodService _goodService;

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public ConstructionSiteInventoryInitializer(IGoodService goodService, InventoryInitializerFactory inventoryInitializerFactory)
	{
		_goodService = goodService;
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(ConstructionSite subject, Inventory decorator)
	{
		BuildingSpec component = subject.GetComponent<BuildingSpec>();
		ValidateCostGoods(component);
		List<StorableGoodAmount> list = component.BuildingCost.Select((GoodAmountSpec requiredGood) => new StorableGoodAmount(StorableGood.CreateAsGivable(requiredGood.Id), requiredGood.Amount)).ToList();
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.Create(decorator, list.Sum((StorableGoodAmount good) => good.Amount), InventoryComponentName);
		inventoryInitializer.AddAllowedGoods(list);
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
	}

	private void ValidateCostGoods(BuildingSpec buildingSpec)
	{
		foreach (GoodAmountSpec item in buildingSpec.BuildingCost)
		{
			if (!_goodService.Goods.Contains(item.Id))
			{
				throw new InvalidOperationException("Cost good " + item.Id + " for building " + buildingSpec.Blueprint.Name + " does not exist");
			}
		}
	}
}
