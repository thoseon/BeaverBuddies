using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.Stockpiles;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileGoodSelectionBoxItemsFactory
{
	private readonly GoodSelectionBoxItemFactory _goodSelectionBoxItemFactory;

	private readonly GoodSelectionBoxRowFactory _goodSelectionBoxRowFactory;

	private readonly IGoodService _goodService;

	public StockpileGoodSelectionBoxItemsFactory(GoodSelectionBoxItemFactory goodSelectionBoxItemFactory, GoodSelectionBoxRowFactory goodSelectionBoxRowFactory, IGoodService goodService)
	{
		_goodSelectionBoxItemFactory = goodSelectionBoxItemFactory;
		_goodSelectionBoxRowFactory = goodSelectionBoxRowFactory;
		_goodService = goodService;
	}

	public IEnumerable<GoodSelectionBoxRow> CreateItems(Stockpile stockpile, Action<string> itemAction, VisualElement root)
	{
		Dictionary<string, GoodSelectionBoxRow> dictionary = new Dictionary<string, GoodSelectionBoxRow>();
		foreach (string item2 in stockpile.GetComponent<StockpileDropdownProvider>().Items)
		{
			if (item2 != StockpileOptionsService.NothingSelectedLocKey)
			{
				string goodGroupId = _goodService.GetGood(item2).GoodGroupId;
				GoodSelectionBoxRow orAdd = dictionary.GetOrAdd(goodGroupId, () => _goodSelectionBoxRowFactory.Create(goodGroupId));
				GoodSelectionBoxItem item = _goodSelectionBoxItemFactory.CreateForGood(item2, itemAction);
				orAdd.AddItem(item);
			}
		}
		foreach (GoodSelectionBoxRow item3 in dictionary.Values.OrderBy((GoodSelectionBoxRow row) => row.Order))
		{
			root.Add(item3.Root);
			yield return item3;
		}
	}
}
