using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InventorySystem;
using Timberborn.InventorySystemUI;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

public class ManufactoryInventoryFragment : IEntityPanelFragment
{
	private readonly InventoryRowUpdater _inventoryRowUpdater;

	private readonly VisualElementLoader _visualElementLoader;

	private Manufactory _manufactory;

	private Inventory _inventory;

	private VisualElement _root;

	private ScrollView _inventoryContent;

	private VisualElement _isEmpty;

	private readonly List<InformationalRow> _rows = new List<InformationalRow>();

	public ManufactoryInventoryFragment(InventoryRowUpdater inventoryRowUpdater, VisualElementLoader visualElementLoader)
	{
		_inventoryRowUpdater = inventoryRowUpdater;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/WorkplaceInventoryFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_inventoryContent = _root.Q<ScrollView>("Content");
		_isEmpty = _root.Q<VisualElement>("IsEmpty");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_manufactory = entity.GetComponent<Manufactory>();
		if ((bool)_manufactory)
		{
			_manufactory.RecipeChanged += OnProductionRecipeChanged;
			_inventory = _manufactory.Inventory;
			_inventoryRowUpdater.AddRows(_inventoryContent, _inventory, _rows, _manufactory.CurrentRecipe);
		}
	}

	public void UpdateFragment()
	{
		_inventoryRowUpdater.UpdateRowsVisibility(_root, _isEmpty, _inventory, _rows);
	}

	public void ClearFragment()
	{
		if ((bool)_manufactory)
		{
			_manufactory.RecipeChanged -= OnProductionRecipeChanged;
		}
		_inventoryContent.Clear();
		_rows.Clear();
		_manufactory = null;
		_inventory = null;
	}

	private void OnProductionRecipeChanged(object sender, EventArgs e)
	{
		_inventoryContent.Clear();
		_rows.Clear();
		_inventoryRowUpdater.AddRows(_inventoryContent, _inventory, _rows, _manufactory.CurrentRecipe);
		UpdateFragment();
	}
}
