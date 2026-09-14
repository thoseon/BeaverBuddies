using Timberborn.Localization;
using Timberborn.PowerManagement;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.PowerManagementUI;

internal class ClutchModeToggle
{
	private static readonly string EngagedClass = "clutch-toggle__icon--engaged";

	private static readonly string DisengagedClass = "clutch-toggle__icon--disengaged";

	private static readonly string AutomatedClass = "clutch-toggle__icon--automated";

	private static readonly string EngagedLocKey = "Building.Clutch.Mode.Engaged";

	private static readonly string DisengagedLocKey = "Building.Clutch.Mode.Disengaged";

	private static readonly string AutomatedLocKey = "Automation.Mode.Automated";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	private Clutch _clutch;

	private SliderToggle _sliderToggle;

	public ClutchModeToggle(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public void Initialize(VisualElement parent)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(() => _loc.T(EngagedLocKey), EngagedClass, delegate
		{
			_clutch.SetMode(ClutchMode.Engaged);
		}, () => _clutch.Mode == ClutchMode.Engaged);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(() => _loc.T(DisengagedLocKey), DisengagedClass, delegate
		{
			_clutch.SetMode(ClutchMode.Disengaged);
		}, () => _clutch.Mode == ClutchMode.Disengaged);
		SliderToggleItem sliderToggleItem3 = SliderToggleItem.Create(() => _loc.T(AutomatedLocKey), AutomatedClass, delegate
		{
			_clutch.SetMode(ClutchMode.Automated);
		}, () => _clutch.Mode == ClutchMode.Automated);
		_sliderToggle = _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2, sliderToggleItem3);
	}

	public void Show(Clutch clutch)
	{
		_clutch = clutch;
	}

	public void Update()
	{
		if ((bool)_clutch)
		{
			_sliderToggle.Update();
		}
	}

	public void Clear()
	{
		_clutch = null;
	}
}
