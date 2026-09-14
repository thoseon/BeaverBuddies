using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.WaterSourceSystemUI;

public class WaterSourceRegulatorToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	public WaterSourceRegulatorToggleFactory(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public WaterSourceRegulatorToggle Create(VisualElement parent, Label label)
	{
		WaterSourceRegulatorToggle waterSourceRegulatorToggle = new WaterSourceRegulatorToggle(_sliderToggleFactory, _loc);
		waterSourceRegulatorToggle.Initialize(parent, label);
		return waterSourceRegulatorToggle;
	}
}
