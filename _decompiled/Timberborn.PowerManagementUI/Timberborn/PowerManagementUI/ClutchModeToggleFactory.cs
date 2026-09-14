using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.PowerManagementUI;

internal class ClutchModeToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	public ClutchModeToggleFactory(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public ClutchModeToggle Create(VisualElement parent)
	{
		ClutchModeToggle clutchModeToggle = new ClutchModeToggle(_sliderToggleFactory, _loc);
		clutchModeToggle.Initialize(parent);
		return clutchModeToggle;
	}
}
