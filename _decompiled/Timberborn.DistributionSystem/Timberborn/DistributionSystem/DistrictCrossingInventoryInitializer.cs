using Timberborn.Goods;
using Timberborn.InventoryNeedSystem;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DistributionSystem;

internal class DistrictCrossingInventoryInitializer : IDedicatedDecoratorInitializer<DistrictCrossingInventory, Inventory>
{
	public static readonly string InventoryComponentName = "DistrictCrossing";

	private static readonly int DistrictCrossingCapacity = 30;

	private readonly IGoodService _goodService;

	private readonly InventoryNeedBehaviorInitializer _inventoryNeedBehaviorInitializer;

	private readonly InventoryInitializerFactory _inventoryInitializerFactory;

	public DistrictCrossingInventoryInitializer(IGoodService goodService, InventoryNeedBehaviorInitializer inventoryNeedBehaviorInitializer, InventoryInitializerFactory inventoryInitializerFactory)
	{
		_goodService = goodService;
		_inventoryNeedBehaviorInitializer = inventoryNeedBehaviorInitializer;
		_inventoryInitializerFactory = inventoryInitializerFactory;
	}

	public void Initialize(DistrictCrossingInventory subject, Inventory decorator)
	{
		InventoryInitializer inventoryInitializer = _inventoryInitializerFactory.CreateWithUnlimitedCapacity(decorator, InventoryComponentName);
		AllowEveryGoodAsTakeable(inventoryInitializer);
		inventoryInitializer.HasPublicOutput();
		inventoryInitializer.SetIgnorableCapacity();
		inventoryInitializer.Initialize();
		subject.InitializeInventory(decorator);
		_inventoryNeedBehaviorInitializer.AddNeedBehavior(decorator);
	}

	private void AllowEveryGoodAsTakeable(InventoryInitializer inventoryInitializer)
	{
		foreach (string good2 in _goodService.Goods)
		{
			StorableGood storableGood = StorableGood.CreateAsTakeable(good2);
			StorableGoodAmount good = new StorableGoodAmount(storableGood, DistrictCrossingCapacity);
			inventoryInitializer.AddAllowedGood(good);
		}
	}
}
