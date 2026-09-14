using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.Hauling;
using Timberborn.InventorySystem;

namespace Timberborn.Workshops;

public class FillInputHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private readonly InventoryFillCalculator _inventoryFillCalculator;

	private BlockableObject _blockableObject;

	private Inventories _inventories;

	private FillInputWorkplaceBehavior _fillInputWorkplaceBehavior;

	public FillInputHaulBehaviorProvider(InventoryFillCalculator inventoryFillCalculator)
	{
		_inventoryFillCalculator = inventoryFillCalculator;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_inventories = GetComponent<Inventories>();
		_fillInputWorkplaceBehavior = GetComponent<FillInputWorkplaceBehavior>();
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		foreach (Inventory enabledInventory in _inventories.EnabledInventories)
		{
			if (_blockableObject.IsUnblocked)
			{
				float weight = 1f - _inventoryFillCalculator.GetInputFillPercentage(enabledInventory);
				weightedBehaviors.Add(new WeightedBehavior(weight, _fillInputWorkplaceBehavior));
			}
		}
	}
}
