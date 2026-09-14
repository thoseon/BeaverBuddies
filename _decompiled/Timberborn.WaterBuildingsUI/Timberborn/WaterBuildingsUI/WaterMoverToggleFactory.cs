using Timberborn.Goods;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class WaterMoverToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly IGoodService _goodService;

	private readonly ILoc _loc;

	public WaterMoverToggleFactory(SliderToggleFactory sliderToggleFactory, IGoodService goodService, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_goodService = goodService;
		_loc = loc;
	}

	public WaterMoverToggle Create(VisualElement parent)
	{
		WaterMoverToggle waterMoverToggle = new WaterMoverToggle(_sliderToggleFactory, _goodService, _loc);
		waterMoverToggle.Initialize(parent);
		return waterMoverToggle;
	}
}
