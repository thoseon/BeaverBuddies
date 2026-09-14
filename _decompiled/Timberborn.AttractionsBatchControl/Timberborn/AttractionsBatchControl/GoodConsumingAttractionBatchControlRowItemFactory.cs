using Timberborn.BatchControl;
using Timberborn.EntitySystem;
using Timberborn.GoodConsumingBuildingSystem;
using Timberborn.InventorySystemBatchControl;

namespace Timberborn.AttractionsBatchControl;

internal class GoodConsumingAttractionBatchControlRowItemFactory
{
	private readonly InventoryCapacityBatchControlRowItemFactory _inventoryCapacityBatchControlRowItemFactory;

	public GoodConsumingAttractionBatchControlRowItemFactory(InventoryCapacityBatchControlRowItemFactory inventoryCapacityBatchControlRowItemFactory)
	{
		_inventoryCapacityBatchControlRowItemFactory = inventoryCapacityBatchControlRowItemFactory;
	}

	public IBatchControlRowItem Create(EntityComponent entity)
	{
		GoodConsumingBuilding component = entity.GetComponent<GoodConsumingBuilding>();
		if (component != null)
		{
			return _inventoryCapacityBatchControlRowItemFactory.Create(component.Inventory);
		}
		return null;
	}
}
