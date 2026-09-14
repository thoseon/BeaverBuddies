using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class GateToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	public GateToggleFactory(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public GateToggle Create(VisualElement parent, Label label)
	{
		GateToggle gateToggle = new GateToggle(_sliderToggleFactory, _loc);
		gateToggle.Initialize(parent, label);
		return gateToggle;
	}
}
