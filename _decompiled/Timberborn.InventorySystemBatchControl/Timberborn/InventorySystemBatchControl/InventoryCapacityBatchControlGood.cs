using Timberborn.InventorySystem;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemBatchControl;

internal class InventoryCapacityBatchControlGood
{
	private readonly Label _capacityAmount;

	private readonly Inventory _inventory;

	private readonly string _goodId;

	public InventoryCapacityBatchControlGood(Label capacityAmount, Inventory inventory, string goodId)
	{
		_capacityAmount = capacityAmount;
		_inventory = inventory;
		_goodId = goodId;
	}

	public void UpdateGoodAmount()
	{
		_capacityAmount.text = _inventory.AmountInStock(_goodId).ToString();
	}
}
