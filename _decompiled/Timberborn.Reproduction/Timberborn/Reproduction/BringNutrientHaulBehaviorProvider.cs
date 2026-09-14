using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.Hauling;
using Timberborn.InventorySystem;

namespace Timberborn.Reproduction;

public class BringNutrientHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private readonly InventoryFillCalculator _inventoryFillCalculator;

	private BreedingPod _breedingPod;

	private BlockableObject _blockableObject;

	private BringNutrientWorkplaceBehavior _bringNutrientWorkplaceBehavior;

	public BringNutrientHaulBehaviorProvider(InventoryFillCalculator inventoryFillCalculator)
	{
		_inventoryFillCalculator = inventoryFillCalculator;
	}

	public void Awake()
	{
		_breedingPod = GetComponent<BreedingPod>();
		_blockableObject = GetComponent<BlockableObject>();
		_bringNutrientWorkplaceBehavior = GetComponent<BringNutrientWorkplaceBehavior>();
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		if ((bool)_breedingPod && _blockableObject.IsUnblocked)
		{
			Inventory inventory = _breedingPod.Inventory;
			float weight = 1f - _inventoryFillCalculator.GetInputFillPercentage(inventory);
			weightedBehaviors.Add(new WeightedBehavior(weight, _bringNutrientWorkplaceBehavior));
		}
	}
}
