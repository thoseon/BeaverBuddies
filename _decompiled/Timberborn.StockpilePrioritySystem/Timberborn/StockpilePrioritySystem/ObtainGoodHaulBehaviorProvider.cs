using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.Hauling;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;

namespace Timberborn.StockpilePrioritySystem;

internal class ObtainGoodHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private readonly InventoryFillCalculator _inventoryFillCalculator;

	private GoodObtainer _goodObtainer;

	private BlockableObject _blockableObject;

	private Inventory _inventory;

	private ObtainGoodWorkplaceBehavior _obtainGoodWorkplaceBehavior;

	public ObtainGoodHaulBehaviorProvider(InventoryFillCalculator inventoryFillCalculator)
	{
		_inventoryFillCalculator = inventoryFillCalculator;
	}

	public void Awake()
	{
		_goodObtainer = GetComponent<GoodObtainer>();
		_blockableObject = GetComponent<BlockableObject>();
		_inventory = GetComponent<Stockpile>().Inventory;
		_obtainGoodWorkplaceBehavior = GetComponent<ObtainGoodWorkplaceBehavior>();
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		if (_goodObtainer.IsObtaining && _blockableObject.IsUnblocked)
		{
			float weight = 1f - _inventoryFillCalculator.GetInputFillPercentage(_inventory);
			weightedBehaviors.Add(new WeightedBehavior(weight, _obtainGoodWorkplaceBehavior));
		}
	}
}
