using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilePriorityUISystem;

public class StockpilePriorityToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	public StockpilePriorityToggleFactory(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public StockpilePriorityToggle Create(VisualElement parent)
	{
		return CreateBindable(parent, null);
	}

	public StockpilePriorityToggle CreateBindable(VisualElement parent, string toggleBindingKey)
	{
		StockpilePriorityToggle stockpilePriorityToggle = new StockpilePriorityToggle(_sliderToggleFactory, _loc);
		stockpilePriorityToggle.Initialize(parent, toggleBindingKey);
		return stockpilePriorityToggle;
	}
}
