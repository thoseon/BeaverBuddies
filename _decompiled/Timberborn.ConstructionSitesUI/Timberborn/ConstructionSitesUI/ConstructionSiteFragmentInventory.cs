using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.InventorySystem;
using Timberborn.InventorySystemUI;
using UnityEngine.UIElements;

namespace Timberborn.ConstructionSitesUI;

public class ConstructionSiteFragmentInventory
{
	private readonly InformationalRowsFactory _informationalRowsFactory;

	private Inventory _inventory;

	private VisualElement _inventoryRoot;

	private ScrollView _inventoryContent;

	private readonly List<InformationalRow> _rows = new List<InformationalRow>();

	public ConstructionSiteFragmentInventory(InformationalRowsFactory informationalRowsFactory)
	{
		_informationalRowsFactory = informationalRowsFactory;
	}

	public void InitializeFragment(VisualElement root)
	{
		_inventoryRoot = root.Q<VisualElement>("ConstructionSiteInventoryFragment");
		_inventoryContent = _inventoryRoot.Q<ScrollView>("Content");
	}

	public void ShowFragment(Inventory inventory)
	{
		_inventory = inventory;
		_rows.AddRange(_informationalRowsFactory.CreateRowsWithLimits(_inventory, _inventoryContent));
	}

	public void ClearFragment()
	{
		_inventoryContent.Clear();
		_rows.Clear();
		_inventory = null;
	}

	public void UpdateFragment()
	{
		if ((bool)_inventory && _inventory.Enabled)
		{
			_inventoryRoot.ToggleDisplayStyle(_rows.Count > 0);
			{
				foreach (InformationalRow row in _rows)
				{
					row.ShowUpdated();
				}
				return;
			}
		}
		_inventoryRoot.ToggleDisplayStyle(visible: false);
	}
}
