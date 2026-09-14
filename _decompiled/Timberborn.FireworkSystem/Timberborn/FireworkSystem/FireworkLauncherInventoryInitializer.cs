using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.FireworkSystem;

internal class FireworkLauncherInventoryInitializer : IDedicatedDecoratorInitializer<FireworkLauncher, Inventory>
{
	private static readonly string InventoryComponentName = "FireworkLauncher";

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public FireworkLauncherInventoryInitializer(InventoryInitializerFactory inventoryInitializerFactory)
	{
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(FireworkLauncher subject, Inventory decorator)
	{
		FireworkLauncherSpec component = subject.GetComponent<FireworkLauncherSpec>();
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.Create(decorator, component.GoodAmount, InventoryComponentName);
		StorableGood storableGood = StorableGood.CreateAsGivable(component.GoodId);
		inventoryInitializer.AddAllowedGood(new StorableGoodAmount(storableGood, component.GoodAmount));
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
	}
}
