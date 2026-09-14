using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class GoodSelectionBoxRow
{
	private readonly VisualElement _itemsRoot;

	private readonly List<GoodSelectionBoxItem> _items = new List<GoodSelectionBoxItem>();

	public VisualElement Root { get; }

	public int Order { get; }

	public GoodSelectionBoxRow(VisualElement root, int order, VisualElement itemsRoot)
	{
		Root = root;
		Order = order;
		_itemsRoot = itemsRoot;
	}

	public void AddItem(GoodSelectionBoxItem item)
	{
		_items.Add(item);
		_itemsRoot.Add(item.Root);
	}

	public void Update()
	{
		for (int i = 0; i < _items.Count; i++)
		{
			_items[i].Update();
		}
	}

	public void UpdateSelectedState(string selectedGoodId)
	{
		for (int i = 0; i < _items.Count; i++)
		{
			_items[i].UpdateSelectedState(selectedGoodId);
		}
	}
}
