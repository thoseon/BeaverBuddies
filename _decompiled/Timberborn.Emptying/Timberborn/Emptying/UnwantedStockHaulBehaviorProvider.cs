using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Hauling;
using Timberborn.InventorySystem;

namespace Timberborn.Emptying;

public class UnwantedStockHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private static readonly float UnwantedStockWeight = 0.5f;

	private Inventories _inventories;

	private RemoveUnwantedStockWorkplaceBehavior _removeUnwantedStockWorkplaceBehavior;

	public void Awake()
	{
		_inventories = GetComponent<Inventories>();
		_removeUnwantedStockWorkplaceBehavior = GetComponent<RemoveUnwantedStockWorkplaceBehavior>();
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		foreach (Inventory enabledInventory in _inventories.EnabledInventories)
		{
			if (enabledInventory.HasUnwantedStock)
			{
				weightedBehaviors.Add(new WeightedBehavior(UnwantedStockWeight, _removeUnwantedStockWorkplaceBehavior));
			}
		}
	}
}
