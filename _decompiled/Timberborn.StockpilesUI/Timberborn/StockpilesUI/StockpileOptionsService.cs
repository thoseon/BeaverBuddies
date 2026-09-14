using Timberborn.Goods;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

public class StockpileOptionsService
{
	public static readonly string NothingSelectedLocKey = "Inventory.NothingSelected";

	private readonly IGoodService _goodService;

	private readonly ILoc _loc;

	public StockpileOptionsService(IGoodService goodService, ILoc loc)
	{
		_goodService = goodService;
		_loc = loc;
	}

	public void UpdateItem(Label text, Image icon, string key)
	{
		text.text = GetItemDisplayText(key);
		icon.sprite = GetItemIcon(key);
	}

	public string GetItemDisplayText(string value)
	{
		if (!(value == NothingSelectedLocKey))
		{
			return _goodService.GetGood(value).PluralDisplayName.Value;
		}
		return _loc.T(NothingSelectedLocKey);
	}

	public Sprite GetItemIcon(string key)
	{
		if (!(key == NothingSelectedLocKey))
		{
			return _goodService.GetGood(key).IconSmall.Value;
		}
		return null;
	}
}
