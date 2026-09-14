using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.Hauling;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;

namespace Timberborn.StockpilePrioritySystem;

public class SupplyGoodHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private readonly InventoryFillCalculator _inventoryFillCalculator;

	private BlockableObject _blockableObject;

	private GoodSupplier _goodSupplier;

	private SupplyGoodWorkplaceBehavior _supplyGoodWorkplaceBehavior;

	private Inventory _inventory;

	public SupplyGoodHaulBehaviorProvider(InventoryFillCalculator inventoryFillCalculator)
	{
		_inventoryFillCalculator = inventoryFillCalculator;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_goodSupplier = GetComponent<GoodSupplier>();
		_supplyGoodWorkplaceBehavior = GetComponent<SupplyGoodWorkplaceBehavior>();
		_inventory = GetComponent<Stockpile>().Inventory;
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		if (_goodSupplier.IsSupplying && _blockableObject.IsUnblocked)
		{
			float inputFillPercentage = _inventoryFillCalculator.GetInputFillPercentage(_inventory);
			weightedBehaviors.Add(new WeightedBehavior(inputFillPercentage, _supplyGoodWorkplaceBehavior));
		}
	}
}
