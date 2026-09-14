using System;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class PowerMeterFragment : IEntityPanelFragment
{
	private static readonly string PowerMeterModeLocKeyPrefix = "Building.PowerMeter.Mode.";

	private static readonly float PercentThresholdStep = 0.01f;

	private readonly Phrase _measurementPhrase = Phrase.New("Automation.Measurement").FormatPower<int>();

	private readonly Phrase _percentMeasurementPhrase = Phrase.New("Automation.Measurement").FormatPercentRounded();

	private readonly Phrase _percentThresholdPhrase = Phrase.New("Automation.Threshold").FormatPercentRounded();

	private readonly VisualElementLoader _visualElementLoader;

	private readonly NumericComparisonModeDropdownFactory _numericComparisonModeDropdownFactory;

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	private VisualElement _root;

	private Dropdown _modeDropdown;

	private EnumDropdownProvider<PowerMeterMode> _powerMeterModeDropdownProvider;

	private Dropdown _comparisonModeDropdown;

	private EnumDropdownProvider<NumericComparisonMode> _comparisonModeDropdownProvider;

	private Label _measurementLabel;

	private IntegerField _intThresholdField;

	private Label _percentThresholdLabel;

	private PreciseSlider _percentThresholdSlider;

	private PowerMeter _powerMeter;

	public PowerMeterFragment(VisualElementLoader visualElementLoader, NumericComparisonModeDropdownFactory numericComparisonModeDropdownFactory, EnumDropdownProviderFactory enumDropdownProviderFactory, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_numericComparisonModeDropdownFactory = numericComparisonModeDropdownFactory;
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_dropdownItemsSetter = dropdownItemsSetter;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/PowerMeterFragment");
		_root.ToggleDisplayStyle(visible: false);
		_intThresholdField = _root.Q<IntegerField>("IntThreshold");
		_intThresholdField.RegisterValueChangedCallback(OnIntThresholdChanged);
		_intThresholdField.isDelayed = true;
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_powerMeterModeDropdownProvider = _enumDropdownProviderFactory.CreateLocalized(() => _powerMeter.Mode, SetMode, PowerMeterModeLocKeyPrefix);
		_comparisonModeDropdown = _root.Q<Dropdown>("ComparisonMode");
		_comparisonModeDropdownProvider = _numericComparisonModeDropdownFactory.Create(() => _powerMeter.ComparisonMode, delegate(NumericComparisonMode comparisonMode)
		{
			_powerMeter.SetComparisonMode(comparisonMode);
		});
		_measurementLabel = _root.Q<Label>("Measurement");
		_percentThresholdLabel = _root.Q<Label>("PercentThresholdLabel");
		_percentThresholdSlider = _root.Q<PreciseSlider>("PercentThresholdSlider");
		_percentThresholdSlider.SetValueChangedCallback(OnPercentThresholdChanged);
		_percentThresholdSlider.SetStepWithoutNotify(PercentThresholdStep);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_powerMeter = entity.GetComponent<PowerMeter>();
		if ((bool)_powerMeter)
		{
			_root.ToggleDisplayStyle(visible: true);
			_dropdownItemsSetter.SetItems(_modeDropdown, _powerMeterModeDropdownProvider);
			_dropdownItemsSetter.SetItems(_comparisonModeDropdown, _comparisonModeDropdownProvider);
		}
	}

	public void ClearFragment()
	{
		_powerMeter = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_powerMeter)
		{
			_percentThresholdLabel.ToggleDisplayStyle(_powerMeter.IsPercentThreshold);
			_percentThresholdSlider.ToggleDisplayStyle(_powerMeter.IsPercentThreshold);
			_intThresholdField.ToggleDisplayStyle(!_powerMeter.IsPercentThreshold);
			if (_powerMeter.IsPercentThreshold)
			{
				_percentThresholdSlider.UpdateValuesWithoutNotify(_powerMeter.PercentThreshold, 0f, 1f);
				_percentThresholdSlider.SetMarker(_powerMeter.PercentMeasurement);
				_percentThresholdLabel.text = _loc.T(_percentThresholdPhrase, _powerMeter.PercentThreshold);
				_measurementLabel.text = _loc.T(_percentMeasurementPhrase, _powerMeter.PercentMeasurement);
			}
			else
			{
				_intThresholdField.SetValueWithoutNotify(_powerMeter.IntThreshold);
				_measurementLabel.text = _loc.T(_measurementPhrase, _powerMeter.IntMeasurement);
			}
		}
	}

	private void SetMode(PowerMeterMode mode)
	{
		_powerMeter.SetMode(mode);
	}

	private void OnIntThresholdChanged(ChangeEvent<int> evt)
	{
		int num = ClampIntThreshold(evt.newValue);
		_intThresholdField.SetValueWithoutNotify(num);
		_powerMeter.SetIntThreshold(num);
	}

	private void OnPercentThresholdChanged(float value)
	{
		_powerMeter.SetPercentThreshold(value);
	}

	private int ClampIntThreshold(int value)
	{
		return _powerMeter.Mode switch
		{
			PowerMeterMode.Supply => Math.Clamp(value, 0, int.MaxValue), 
			PowerMeterMode.Demand => Math.Clamp(value, 0, int.MaxValue), 
			PowerMeterMode.Surplus => value, 
			_ => throw new ArgumentOutOfRangeException(_powerMeter.Mode.ToString()), 
		};
	}
}
