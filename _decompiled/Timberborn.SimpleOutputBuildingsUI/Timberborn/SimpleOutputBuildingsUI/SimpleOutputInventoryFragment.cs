using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InventorySystemUI;
using Timberborn.SimpleOutputBuildings;
using UnityEngine.UIElements;

namespace Timberborn.SimpleOutputBuildingsUI;

internal class SimpleOutputInventoryFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private InventoryFragment _inventoryFragment;

	private SimpleOutputInventory _simpleOutputInventory;

	private VisualElement _root;

	public SimpleOutputInventoryFragment(VisualElementLoader visualElementLoader, InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory)
	{
		_visualElementLoader = visualElementLoader;
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/SimpleOutputInventoryFragment");
		_root.ToggleDisplayStyle(visible: false);
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(_root).ShowRowLimit().ShowNoGoodInStockMessage()
			.Build();
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_simpleOutputInventory = entity.GetComponent<SimpleOutputInventory>();
		if ((bool)_simpleOutputInventory && (bool)entity.GetComponent<SimpleOutputInventoryFragmentEnabler>())
		{
			_inventoryFragment.ShowFragment(_simpleOutputInventory.Inventory);
		}
		else
		{
			_simpleOutputInventory = null;
		}
	}

	public void ClearFragment()
	{
		_simpleOutputInventory = null;
		_inventoryFragment.ClearFragment();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_simpleOutputInventory && _simpleOutputInventory.Enabled)
		{
			_inventoryFragment.UpdateFragment();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
