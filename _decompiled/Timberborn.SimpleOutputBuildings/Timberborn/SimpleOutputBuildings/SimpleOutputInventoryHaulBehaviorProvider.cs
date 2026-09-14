using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.InventorySystem;

namespace Timberborn.SimpleOutputBuildings;

public class SimpleOutputInventoryHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private readonly InventoryFillCalculator _inventoryFillCalculator;

	private EmptyOutputWorkplaceBehavior _emptyOutputWorkplaceBehavior;

	private BlockableObject _blockableObject;

	private Inventory _inventory;

	public SimpleOutputInventoryHaulBehaviorProvider(InventoryFillCalculator inventoryFillCalculator)
	{
		_inventoryFillCalculator = inventoryFillCalculator;
	}

	public void Awake()
	{
		_emptyOutputWorkplaceBehavior = GetComponent<EmptyOutputWorkplaceBehavior>();
		_blockableObject = GetComponent<BlockableObject>();
		_inventory = GetComponent<SimpleOutputInventory>().Inventory;
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		if (_inventory.Enabled && _blockableObject.IsUnblocked)
		{
			float inStockOutputFillPercentage = _inventoryFillCalculator.GetInStockOutputFillPercentage(_inventory);
			weightedBehaviors.Add(new WeightedBehavior(inStockOutputFillPercentage, _emptyOutputWorkplaceBehavior));
		}
	}
}
