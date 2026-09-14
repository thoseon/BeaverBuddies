using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.InventorySystem;

namespace Timberborn.Workshops;

public class ManufactoryHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private readonly InventoryFillCalculator _inventoryFillCalculator;

	private Manufactory _manufactory;

	private BlockableObject _blockableObject;

	private Inventories _inventories;

	private FillInputWorkplaceBehavior _fillInputWorkplaceBehavior;

	private EmptyOutputWorkplaceBehavior _emptyOutputWorkplaceBehavior;

	public ManufactoryHaulBehaviorProvider(InventoryFillCalculator inventoryFillCalculator)
	{
		_inventoryFillCalculator = inventoryFillCalculator;
	}

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		_blockableObject = GetComponent<BlockableObject>();
		_inventories = GetComponent<Inventories>();
		_fillInputWorkplaceBehavior = GetComponent<FillInputWorkplaceBehavior>();
		_emptyOutputWorkplaceBehavior = GetComponent<EmptyOutputWorkplaceBehavior>();
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		if (!_manufactory || !_manufactory.HasCurrentRecipe || !_blockableObject.IsUnblocked)
		{
			return;
		}
		foreach (Inventory enabledInventory in _inventories.EnabledInventories)
		{
			if (enabledInventory.IsInput)
			{
				float num = 1f - _inventoryFillCalculator.GetInputFillPercentage(enabledInventory);
				if (num > 0f)
				{
					weightedBehaviors.Add(new WeightedBehavior(num, _fillInputWorkplaceBehavior));
				}
			}
			if (enabledInventory.IsOutput)
			{
				float outputFillPercentage = _inventoryFillCalculator.GetOutputFillPercentage(enabledInventory);
				if (outputFillPercentage > 0f)
				{
					weightedBehaviors.Add(new WeightedBehavior(outputFillPercentage, _emptyOutputWorkplaceBehavior));
				}
			}
		}
	}
}
