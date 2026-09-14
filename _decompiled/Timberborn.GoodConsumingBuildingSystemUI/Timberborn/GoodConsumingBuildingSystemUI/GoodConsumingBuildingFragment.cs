using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GoodConsumingBuildingSystem;
using Timberborn.InventorySystemUI;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.GoodConsumingBuildingSystemUI;

public class GoodConsumingBuildingFragment : IEntityPanelFragment
{
	private static readonly string RemainingLocKey = "GoodConsuming.SupplyRemaining";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private readonly ILoc _loc;

	private InventoryFragment _inventoryFragment;

	private GoodConsumingBuilding _goodConsumingBuilding;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _hoursLeftBar;

	private Label _hoursLeft;

	public GoodConsumingBuildingFragment(VisualElementLoader visualElementLoader, InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/GoodConsumingBuildingFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(_root).ShowRowLimit().ShowEmptyRows()
			.Build();
		_hoursLeftBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_hoursLeft = _root.Q<Label>("HoursLeft");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_goodConsumingBuilding = entity.GetComponent<GoodConsumingBuilding>();
		if ((bool)(BaseComponent)(object)_goodConsumingBuilding)
		{
			_inventoryFragment.ShowFragment(_goodConsumingBuilding.Inventory);
		}
	}

	public void ClearFragment()
	{
		_goodConsumingBuilding = null;
		_inventoryFragment.ClearFragment();
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_goodConsumingBuilding && ((BaseComponent)(object)_goodConsumingBuilding).Enabled)
		{
			_inventoryFragment.UpdateFragment();
			UpdateProgressBar();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateProgressBar()
	{
		float num = _goodConsumingBuilding.HoursUntilNoSupply();
		_hoursLeftBar.SetProgress(num / _goodConsumingBuilding.MaximumWorkingTime);
		_hoursLeft.text = _loc.T(RemainingLocKey, $"{num:F1}");
	}
}
