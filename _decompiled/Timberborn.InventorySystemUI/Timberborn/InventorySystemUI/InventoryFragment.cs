using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.InventorySystem;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemUI;

public class InventoryFragment
{
	public class Builder
	{
		private readonly VisualElementLoader _visualElementLoader;

		private readonly InformationalRowsFactory _informationalRowsFactory;

		private readonly VisualElement _root;

		private bool _showEmptyRows;

		private bool _showRowLimit;

		private bool _showNoGoodInStockMessage;

		private string _simpleHeaderLocKey;

		public Builder(VisualElementLoader visualElementLoader, InformationalRowsFactory informationalRowsFactory, VisualElement root)
		{
			_visualElementLoader = visualElementLoader;
			_informationalRowsFactory = informationalRowsFactory;
			_root = root;
		}

		public Builder ShowEmptyRows()
		{
			_showEmptyRows = true;
			return this;
		}

		public Builder ShowRowLimit()
		{
			_showRowLimit = true;
			return this;
		}

		public Builder ShowNoGoodInStockMessage()
		{
			_showNoGoodInStockMessage = true;
			return this;
		}

		public InventoryFragment Build()
		{
			string elementName = "Game/EntityPanel/InventoryFragment";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			InventoryFragment result = new InventoryFragment(_informationalRowsFactory, _root, visualElement.Q<ScrollView>("Content"), _showEmptyRows, _showRowLimit, visualElement.Q<Label>("IsEmpty"), _showNoGoodInStockMessage);
			_root.Add(visualElement);
			_root.ToggleDisplayStyle(visible: false);
			return result;
		}
	}

	private readonly InformationalRowsFactory _informationalRowsFactory;

	private readonly VisualElement _root;

	private readonly ScrollView _rowsRoot;

	private readonly bool _showEmptyRows;

	private readonly Label _noGoodInStockMessage;

	private readonly bool _showNoGoodInStockMessage;

	private readonly List<InformationalRow> _rows = new List<InformationalRow>();

	private Inventory _inventory;

	private readonly bool _showRowLimit;

	private InventoryFragment(InformationalRowsFactory informationalRowsFactory, VisualElement root, ScrollView rowsRoot, bool showEmptyRows, bool showRowLimit, Label noGoodInStockMessage, bool showNoGoodInStockMessage)
	{
		_informationalRowsFactory = informationalRowsFactory;
		_root = root;
		_rowsRoot = rowsRoot;
		_showEmptyRows = showEmptyRows;
		_showRowLimit = showRowLimit;
		_noGoodInStockMessage = noGoodInStockMessage;
		_showNoGoodInStockMessage = showNoGoodInStockMessage;
	}

	public void ShowFragment(Inventory inventory)
	{
		_inventory = inventory;
		bool flag = inventory.HasComponent<InventoryLimitRowHiderSpec>();
		IEnumerable<InformationalRow> collection = ((_showRowLimit && !flag) ? _informationalRowsFactory.CreateRowsWithLimits(_inventory, _rowsRoot) : _informationalRowsFactory.CreateRowsWithoutLimits(_inventory, _rowsRoot));
		_rows.AddRange(collection);
	}

	public void ClearFragment()
	{
		_rowsRoot.Clear();
		_rows.Clear();
		_inventory = null;
	}

	public void UpdateFragment()
	{
		if ((bool)_inventory && _inventory.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			UpdateInventoryRows();
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateInventoryRows()
	{
		bool flag = true;
		foreach (InformationalRow row in _rows)
		{
			if (_showEmptyRows || row.CurrentAmount > 0)
			{
				row.ShowUpdated();
				flag = false;
			}
			else
			{
				row.Hide();
			}
		}
		_noGoodInStockMessage.ToggleDisplayStyle(_showNoGoodInStockMessage & flag);
		_rowsRoot.ToggleDisplayStyle(!_showNoGoodInStockMessage || (_showNoGoodInStockMessage && !flag));
	}
}
