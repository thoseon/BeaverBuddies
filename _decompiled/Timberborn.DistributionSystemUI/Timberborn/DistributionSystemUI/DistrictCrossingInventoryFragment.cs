using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.InventorySystemUI;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemUI;

internal class DistrictCrossingInventoryFragment : IEntityPanelFragment
{
	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private InventoryFragment _inventoryFragment;

	private DistrictCrossingInventory _districtCrossingInventory;

	private VisualElement _root;

	public DistrictCrossingInventoryFragment(InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory, VisualElementLoader visualElementLoader)
	{
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/DistrictCrossingInventoryFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_root.ToggleDisplayStyle(visible: false);
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(_root).ShowRowLimit().ShowNoGoodInStockMessage()
			.Build();
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_districtCrossingInventory = entity.GetComponent<DistrictCrossingInventory>();
		if ((bool)_districtCrossingInventory)
		{
			_root.ToggleDisplayStyle(visible: true);
			_inventoryFragment.ShowFragment(_districtCrossingInventory.Inventory);
		}
	}

	public void ClearFragment()
	{
		_districtCrossingInventory = null;
		_inventoryFragment.ClearFragment();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_districtCrossingInventory)
		{
			_inventoryFragment.UpdateFragment();
		}
	}
}
