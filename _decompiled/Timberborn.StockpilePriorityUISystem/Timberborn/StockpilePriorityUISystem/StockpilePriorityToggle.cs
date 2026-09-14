using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.StockpilePrioritySystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilePriorityUISystem;

public class StockpilePriorityToggle
{
	private static readonly string AcceptClass = "stockpile-priority-toggle__icon--accept";

	private static readonly string EmptyClass = "stockpile-priority-toggle__icon--empty";

	private static readonly string ObtainClass = "stockpile-priority-toggle__icon--obtain";

	private static readonly string SupplyClass = "stockpile-priority-toggle__icon--supply";

	private static readonly string AcceptLocKey = "StockpilePriority.Accept";

	private static readonly string EmptyLocKey = "StockpilePriority.Empty";

	private static readonly string ObtainLocKey = "StockpilePriority.Obtain";

	private static readonly string SupplyLocKey = "StockpilePriority.Supply";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	private SliderToggle _sliderToggle;

	private StockpilePriority _stockpilePriority;

	public StockpilePriorityToggle(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public void Initialize(VisualElement parent, string toggleBindingKey = null)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(() => _loc.T(AcceptLocKey), AcceptClass, delegate
		{
			_stockpilePriority.Accept();
		}, () => _stockpilePriority.IsAcceptActive);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(() => _loc.T(EmptyLocKey), EmptyClass, delegate
		{
			_stockpilePriority.Empty();
		}, () => _stockpilePriority.IsEmptyActive);
		SliderToggleItem sliderToggleItem3 = SliderToggleItem.Create(() => _loc.T(ObtainLocKey), ObtainClass, delegate
		{
			_stockpilePriority.Obtain();
		}, () => _stockpilePriority.IsObtainActive);
		SliderToggleItem sliderToggleItem4 = SliderToggleItem.Create(() => _loc.T(SupplyLocKey), SupplyClass, delegate
		{
			_stockpilePriority.Supply();
		}, () => _stockpilePriority.IsSupplyActive);
		_sliderToggle = (string.IsNullOrWhiteSpace(toggleBindingKey) ? _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem3, sliderToggleItem4, sliderToggleItem2) : _sliderToggleFactory.CreateBindable(parent, toggleBindingKey, sliderToggleItem, sliderToggleItem3, sliderToggleItem4, sliderToggleItem2));
	}

	public void Show(StockpilePriority stockpilePriority)
	{
		_stockpilePriority = stockpilePriority;
	}

	public void Update()
	{
		if (!_sliderToggle.IsBound)
		{
			_sliderToggle.Bind();
		}
		_sliderToggle.Update();
	}

	public void Clear()
	{
		_sliderToggle.Unbind();
		_sliderToggle.Clear();
		_stockpilePriority = null;
	}
}
