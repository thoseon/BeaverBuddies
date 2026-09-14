using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileInventoryDebugFragment : IEntityPanelFragment
{
	private readonly StockpileInventoryFragment _stockpileInventoryFragment;

	private readonly DebugFragmentFactory _debugFragmentFactory;

	private VisualElement _root;

	private SingleGoodAllower _singleGoodAllower;

	private Stockpile _stockpile;

	public StockpileInventoryDebugFragment(StockpileInventoryFragment stockpileInventoryFragment, DebugFragmentFactory debugFragmentFactory)
	{
		_stockpileInventoryFragment = stockpileInventoryFragment;
		_debugFragmentFactory = debugFragmentFactory;
	}

	public VisualElement InitializeFragment()
	{
		DebugFragmentButton debugFragmentButton = new DebugFragmentButton(OnGiveAllButtonClick, "Inventory: Give all");
		_root = _debugFragmentFactory.Create("StockpileInventoryFragment", debugFragmentButton);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_singleGoodAllower = entity.GetComponent<SingleGoodAllower>();
		_stockpile = entity.GetComponent<Stockpile>();
	}

	public void ClearFragment()
	{
		_singleGoodAllower = null;
		_stockpile = null;
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle((bool)_stockpile && (bool)_singleGoodAllower);
	}

	private void OnGiveAllButtonClick()
	{
		if (_singleGoodAllower.HasAllowedGood)
		{
			GiveAll();
		}
		else
		{
			_stockpileInventoryFragment.ShowGoodSelectionBox();
		}
	}

	private void GiveAll()
	{
		Inventory inventory = _stockpile.Inventory;
		string allowedGood = _singleGoodAllower.AllowedGood;
		int amount = inventory.UnreservedCapacity(allowedGood);
		inventory.GiveProduced(new GoodAmount(allowedGood, amount));
	}
}
