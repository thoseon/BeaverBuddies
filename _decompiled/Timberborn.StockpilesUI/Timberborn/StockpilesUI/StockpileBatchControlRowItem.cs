using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.InventorySystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileBatchControlRowItem : IBatchControlRowItem, IInitializableBatchControlItem, IClearableBatchControlRowItem
{
	private readonly Inventory _inventory;

	private readonly SingleGoodAllower _singleGoodAllower;

	private readonly Dropdown _dropdown;

	private readonly Label _capacityAmount;

	private readonly VisualElement _fillGauge;

	public VisualElement Root { get; }

	public StockpileBatchControlRowItem(VisualElement root, Inventory inventory, SingleGoodAllower singleGoodAllower, Label capacityAmount, Dropdown dropdown, VisualElement fillGauge)
	{
		Root = root;
		_inventory = inventory;
		_singleGoodAllower = singleGoodAllower;
		_capacityAmount = capacityAmount;
		_dropdown = dropdown;
		_fillGauge = fillGauge;
	}

	public void InitializeRowItem()
	{
		_inventory.InventoryChanged += OnInventoryChanged;
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		UpdateAmounts();
	}

	public void ClearRowItem()
	{
		_inventory.InventoryChanged -= OnInventoryChanged;
		_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		UpdateAmounts();
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		_dropdown.UpdateSelectedValue();
	}

	private void UpdateAmounts()
	{
		int totalAmountInStock = _inventory.TotalAmountInStock;
		_capacityAmount.text = totalAmountInStock.ToString();
		_fillGauge.SetHeightAsPercent((float)totalAmountInStock / (float)_inventory.Capacity);
	}
}
