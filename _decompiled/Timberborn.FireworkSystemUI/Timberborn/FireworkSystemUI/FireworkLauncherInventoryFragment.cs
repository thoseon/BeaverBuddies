using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.FireworkSystem;
using Timberborn.InventorySystemUI;
using UnityEngine.UIElements;

namespace Timberborn.FireworkSystemUI;

internal class FireworkLauncherInventoryFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private VisualElement _root;

	private InventoryFragment _inventoryFragment;

	private FireworkLauncher _fireworkLauncher;

	public FireworkLauncherInventoryFragment(VisualElementLoader visualElementLoader, InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory)
	{
		_visualElementLoader = visualElementLoader;
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/FireworkLauncherInventoryFragment");
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(_root).ShowEmptyRows().ShowRowLimit()
			.Build();
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_fireworkLauncher = entity.GetComponent<FireworkLauncher>();
		if ((bool)_fireworkLauncher)
		{
			_root.ToggleDisplayStyle(visible: true);
			_inventoryFragment.ShowFragment(_fireworkLauncher.Inventory);
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_fireworkLauncher)
		{
			_inventoryFragment.UpdateFragment();
		}
	}

	public void ClearFragment()
	{
		_fireworkLauncher = null;
		_inventoryFragment.ClearFragment();
		_root.ToggleDisplayStyle(visible: false);
	}
}
