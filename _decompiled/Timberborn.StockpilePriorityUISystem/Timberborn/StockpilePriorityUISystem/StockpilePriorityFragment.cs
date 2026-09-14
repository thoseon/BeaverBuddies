using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.StockpilePrioritySystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilePriorityUISystem;

internal class StockpilePriorityFragment : IEntityPanelFragment
{
	private static readonly string AcceptLongLocKey = "StockpilePriority.Accept.Long";

	private static readonly string EmptyLongLocKey = "StockpilePriority.Empty.Long";

	private static readonly string ObtainLongLocKey = "StockpilePriority.Obtain.Long";

	private static readonly string SupplyLongLocKey = "StockpilePriority.Supply.Long";

	private static readonly string ToggleStockpilePriorityKey = "ToggleStockpilePriority";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly StockpilePriorityToggleFactory _stockpilePriorityToggleFactory;

	private readonly ILoc _loc;

	private StockpilePriority _stockpilePriority;

	private VisualElement _root;

	private StockpilePriorityToggle _sliderToggle;

	private Label _description;

	public StockpilePriorityFragment(VisualElementLoader visualElementLoader, StockpilePriorityToggleFactory stockpilePriorityToggleFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_stockpilePriorityToggleFactory = stockpilePriorityToggleFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/StockpilePriorityFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_description = _root.Q<Label>("Description");
		VisualElement parent = _root.Q<VisualElement>("ToggleWrapper");
		_sliderToggle = _stockpilePriorityToggleFactory.CreateBindable(parent, ToggleStockpilePriorityKey);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		StockpilePriority component = entity.GetComponent<StockpilePriority>();
		if (component != null)
		{
			_stockpilePriority = component;
			_sliderToggle.Show(_stockpilePriority);
		}
	}

	public void ClearFragment()
	{
		_sliderToggle.Clear();
		_stockpilePriority = null;
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle(_stockpilePriority);
		if ((bool)_stockpilePriority)
		{
			_sliderToggle.Update();
			_description.text = GetDescription();
		}
	}

	private string GetDescription()
	{
		if (_stockpilePriority.IsEmptyActive)
		{
			return _loc.T(EmptyLongLocKey);
		}
		if (_stockpilePriority.IsObtainActive)
		{
			return _loc.T(ObtainLongLocKey);
		}
		if (_stockpilePriority.IsSupplyActive)
		{
			return _loc.T(SupplyLongLocKey);
		}
		return _loc.T(AcceptLongLocKey);
	}
}
