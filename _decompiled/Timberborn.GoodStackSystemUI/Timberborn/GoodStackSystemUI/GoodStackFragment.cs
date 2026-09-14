using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GoodStackSystem;
using Timberborn.InventorySystemUI;
using UnityEngine.UIElements;

namespace Timberborn.GoodStackSystemUI;

public class GoodStackFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private InventoryFragment _inventoryFragment;

	private GoodStack _goodStack;

	private VisualElement _root;

	public GoodStackFragment(VisualElementLoader visualElementLoader, InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory)
	{
		_visualElementLoader = visualElementLoader;
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/GoodStackFragment");
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(_root).Build();
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_goodStack = entity.GetComponent<GoodStack>();
		if ((bool)_goodStack)
		{
			_inventoryFragment.ShowFragment(_goodStack.Inventory);
		}
	}

	public void ClearFragment()
	{
		_goodStack = null;
		_inventoryFragment.ClearFragment();
	}

	public void UpdateFragment()
	{
		if ((bool)_goodStack && _goodStack.Enabled)
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
