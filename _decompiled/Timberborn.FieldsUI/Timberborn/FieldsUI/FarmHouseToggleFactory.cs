using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.FieldsUI;

public class FarmHouseToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	public FarmHouseToggleFactory(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public FarmHouseToggle Create(VisualElement parent)
	{
		FarmHouseToggle farmHouseToggle = new FarmHouseToggle(_sliderToggleFactory, _loc);
		farmHouseToggle.Initialize(parent);
		return farmHouseToggle;
	}
}
