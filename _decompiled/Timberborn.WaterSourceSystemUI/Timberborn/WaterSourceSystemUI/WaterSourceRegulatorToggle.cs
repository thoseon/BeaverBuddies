using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.WaterSourceSystem;
using UnityEngine.UIElements;

namespace Timberborn.WaterSourceSystemUI;

public class WaterSourceRegulatorToggle
{
	private static readonly string ClosedClass = "water-source-regulator-toggle__icon--closed";

	private static readonly string OpenedClass = "water-source-regulator-toggle__icon--open";

	private static readonly string AutomatedClass = "water-source-regulator-toggle__icon--automated";

	private static readonly string ToggleClosedLocKey = "Toggle.State.Closed";

	private static readonly string ToggleOpenLocKey = "Toggle.State.Open";

	private static readonly string ToggleAutomatedLocKey = "Automation.Mode.Automated";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	private WaterSourceRegulator _waterSourceRegulator;

	private SliderToggle _sliderToggle;

	private Label _modeLabel;

	public WaterSourceRegulatorToggle(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public void Initialize(VisualElement parent, Label modeLabel)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(() => _loc.T(ToggleClosedLocKey), ClosedClass, delegate
		{
			_waterSourceRegulator.Close();
		}, () => _waterSourceRegulator.ClosedMode);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(() => _loc.T(ToggleOpenLocKey), OpenedClass, delegate
		{
			_waterSourceRegulator.Open();
		}, () => _waterSourceRegulator.OpenMode);
		SliderToggleItem sliderToggleItem3 = SliderToggleItem.Create(() => _loc.T(ToggleAutomatedLocKey), AutomatedClass, delegate
		{
			_waterSourceRegulator.Automate();
		}, () => _waterSourceRegulator.AutomatedMode);
		_sliderToggle = _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2, sliderToggleItem3);
		_modeLabel = modeLabel;
	}

	public void Show(WaterSourceRegulator waterSourceRegulator)
	{
		_waterSourceRegulator = waterSourceRegulator;
	}

	public void Update()
	{
		if (_waterSourceRegulator != null)
		{
			_sliderToggle.Update();
			_modeLabel.text = GetModeLabel();
		}
	}

	public void Clear()
	{
		_waterSourceRegulator = null;
	}

	private string GetModeLabel()
	{
		if (_waterSourceRegulator.OpenMode)
		{
			return _loc.T(ToggleOpenLocKey);
		}
		if (_waterSourceRegulator.ClosedMode)
		{
			return _loc.T(ToggleClosedLocKey);
		}
		if (_waterSourceRegulator.AutomatedMode)
		{
			string text = "(" + _loc.T(_waterSourceRegulator.IsOpen ? ToggleOpenLocKey : ToggleClosedLocKey) + ")";
			return _loc.T(ToggleAutomatedLocKey) + " " + text;
		}
		return string.Empty;
	}
}
