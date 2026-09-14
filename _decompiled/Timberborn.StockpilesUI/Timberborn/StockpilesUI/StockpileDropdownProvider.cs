using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;
using UnityEngine;

namespace Timberborn.StockpilesUI;

internal class StockpileDropdownProvider : BaseComponent, IAwakableComponent, IInitializableEntity, IExtendedDropdownProvider, IDropdownProvider
{
	private readonly StockpileOptionsService _stockpileOptionsService;

	private SingleGoodAllower _singleGoodAllower;

	private Inventory _inventory;

	private readonly List<string> _items = new List<string>();

	public IReadOnlyList<string> Items => _items.AsReadOnlyList();

	public StockpileDropdownProvider(StockpileOptionsService stockpileOptionsService)
	{
		_stockpileOptionsService = stockpileOptionsService;
	}

	public void Awake()
	{
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		_inventory = GetComponent<Stockpile>().Inventory;
	}

	public void InitializeEntity()
	{
		IEnumerable<string> collection = _inventory.AllowedGoods.Select((StorableGoodAmount good) => good.StorableGood.GoodId);
		_items.AddRange(collection);
		_items.Add(StockpileOptionsService.NothingSelectedLocKey);
	}

	public string GetValue()
	{
		return _singleGoodAllower.AllowedGood ?? StockpileOptionsService.NothingSelectedLocKey;
	}

	public void SetValue(string value)
	{
		if (value == StockpileOptionsService.NothingSelectedLocKey)
		{
			_singleGoodAllower.Disallow();
		}
		else
		{
			_singleGoodAllower.Allow(value);
		}
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return _stockpileOptionsService.GetItemDisplayText(value);
	}

	public Sprite GetIcon(string value)
	{
		return _stockpileOptionsService.GetItemIcon(value);
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}
}
