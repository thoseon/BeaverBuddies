using System.Collections.Generic;
using System.Linq;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Reproduction;

public class BreedingPodInventoryInitializer : IDedicatedDecoratorInitializer<BreedingPod, Inventory>
{
	private static readonly string InventoryComponentName = "BreedingPod";

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public BreedingPodInventoryInitializer(InventoryInitializerFactory inventoryInitializerFactory)
	{
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(BreedingPod subject, Inventory decorator)
	{
		List<StorableGoodAmount> list = new List<StorableGoodAmount>();
		BreedingPodSpec component = subject.GetComponent<BreedingPodSpec>();
		foreach (GoodAmountSpec item in component.NutrientsPerCycle)
		{
			StorableGood storableGood = StorableGood.CreateAsGivable(item.Id);
			list.Add(new StorableGoodAmount(storableGood, item.Amount * component.CyclesCapacity));
		}
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.Create(decorator, list.Sum((StorableGoodAmount good) => good.Amount), InventoryComponentName);
		inventoryInitializer.AddAllowedGoods(list);
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
	}
}
