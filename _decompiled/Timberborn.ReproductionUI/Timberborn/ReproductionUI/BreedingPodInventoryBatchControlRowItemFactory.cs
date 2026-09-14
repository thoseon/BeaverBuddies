using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.InventorySystemBatchControl;
using Timberborn.Reproduction;

namespace Timberborn.ReproductionUI;

public class BreedingPodInventoryBatchControlRowItemFactory
{
	private readonly InventoryCapacityBatchControlRowItemFactory _inventoryCapacityBatchControlRowItemFactory;

	public BreedingPodInventoryBatchControlRowItemFactory(InventoryCapacityBatchControlRowItemFactory inventoryCapacityBatchControlRowItemFactory)
	{
		_inventoryCapacityBatchControlRowItemFactory = inventoryCapacityBatchControlRowItemFactory;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		BreedingPod component = entity.GetComponent<BreedingPod>();
		if (!component)
		{
			return null;
		}
		return _inventoryCapacityBatchControlRowItemFactory.Create(component.Inventory);
	}
}
