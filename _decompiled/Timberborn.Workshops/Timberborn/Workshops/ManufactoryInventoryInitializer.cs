using System.Collections.Generic;
using System.Linq;
using Timberborn.Goods;
using Timberborn.InventoryNeedSystem;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Workshops;

public class ManufactoryInventoryInitializer : IDedicatedDecoratorInitializer<Manufactory, Inventory>
{
	private static readonly string InventoryComponentName = "Manufactory";

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	private readonly InventoryNeedBehaviorInitializer _inventoryNeedBehaviorInitializer;

	private readonly RecipeGoodsProcessor _recipeGoodsProcessor;

	public ManufactoryInventoryInitializer(InventoryInitializerFactory inventoryInitializerFactory, InventoryNeedBehaviorInitializer inventoryNeedBehaviorInitializer, RecipeGoodsProcessor recipeGoodsProcessor)
	{
		_inventoryInitializerFactory = inventoryInitializerFactory;
		_inventoryNeedBehaviorInitializer = inventoryNeedBehaviorInitializer;
		_recipeGoodsProcessor = recipeGoodsProcessor;
	}

	public void Initialize(Manufactory subject, Inventory decorator)
	{
		ManufactorySpec component = subject.GetComponent<ManufactorySpec>();
		Dictionary<StorableGood, int> dictionary = _recipeGoodsProcessor.ProcessRecipes(component.ProductionRecipeIds);
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.Create(decorator, dictionary.Values.Sum(), InventoryComponentName);
		inventoryInitializer.AddAllowedGoods(dictionary.Select((KeyValuePair<StorableGood, int> pair) => new StorableGoodAmount(pair.Key, pair.Value)));
		RecipeGoodDisallower component2 = subject.GetComponent<RecipeGoodDisallower>();
		inventoryInitializer.AddGoodDisallower(component2);
		inventoryInitializer.HasPublicOutput();
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
		_inventoryNeedBehaviorInitializer.AddNeedBehavior(decorator);
	}
}
