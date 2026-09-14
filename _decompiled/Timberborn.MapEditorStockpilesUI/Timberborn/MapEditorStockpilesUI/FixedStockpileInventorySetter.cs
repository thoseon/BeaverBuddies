using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;

namespace Timberborn.MapEditorStockpilesUI;

internal class FixedStockpileInventorySetter : BaseComponent, IAwakableComponent, IInitializableEntity, IDuplicable<FixedStockpileInventorySetter>, IDuplicable
{
	private readonly FixedStockpileGoodProvider _fixedStockpileGoodProvider;

	private Stockpile _stockpile;

	private FixedStockpile _fixedStockpile;

	private SingleGoodAllower _singleGoodAllower;

	public FixedStockpileInventorySetter(FixedStockpileGoodProvider fixedStockpileGoodProvider)
	{
		_fixedStockpileGoodProvider = fixedStockpileGoodProvider;
	}

	public void Awake()
	{
		_stockpile = GetComponent<Stockpile>();
		_fixedStockpile = GetComponent<FixedStockpile>();
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
	}

	public void InitializeEntity()
	{
		ValidateAndInitializeInventory();
	}

	public void SetGoodId(string goodId)
	{
		int totalAmountInStock = _stockpile.Inventory.TotalAmountInStock;
		ResetInventoryGood(goodId);
		if (totalAmountInStock > 0)
		{
			_stockpile.Inventory.GiveExisting(new GoodAmount(goodId, totalAmountInStock));
		}
	}

	public void SetAmount(int amount)
	{
		ClearInventory();
		_stockpile.Inventory.GiveExisting(new GoodAmount(_singleGoodAllower.AllowedGood, amount));
	}

	public void DuplicateFrom(FixedStockpileInventorySetter source)
	{
		if (_stockpile.WhitelistedGoodType == source._stockpile.WhitelistedGoodType)
		{
			SetGoodId(source._singleGoodAllower.AllowedGood);
			SetAmount(source._stockpile.Inventory.TotalAmountInStock);
		}
	}

	private void ValidateAndInitializeInventory()
	{
		ImmutableArray<string> goods = _fixedStockpileGoodProvider.GetGoods(_stockpile.WhitelistedGoodType);
		bool hasAllowedGood = _singleGoodAllower.HasAllowedGood;
		if (!hasAllowedGood || !goods.Contains(_singleGoodAllower.AllowedGood))
		{
			ResetInventoryGood(goods.First());
		}
		if (!hasAllowedGood)
		{
			SetAmount(_stockpile.MaxCapacity);
		}
	}

	private void ResetInventoryGood(string goodId)
	{
		ClearInventory();
		_singleGoodAllower.Allow(goodId);
		_fixedStockpile.SetFixedGood(goodId);
	}

	private void ClearInventory()
	{
		foreach (GoodAmount item in _stockpile.Inventory.Stock.ToImmutableArray())
		{
			_stockpile.Inventory.TakeExisting(item);
		}
	}
}
