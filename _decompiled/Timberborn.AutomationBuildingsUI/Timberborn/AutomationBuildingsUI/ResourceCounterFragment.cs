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

internal class ResourceCounterFragment : IEntityPanelFragment
{
	private static readonly float PercentThresholdChangeStep = 0.01f;

	private static readonly string PercentModeLocKey = "Building.ResourceCounter.Mode.Percent";

	private static readonly string QuantityModeLocKey = "Building.ResourceCounter.Mode.Quantity";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly NumericComparisonModeDropdownFactory _numericComparisonModeDropdownFactory;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly RadioToggleFactory _radioToggleFactory;

	private readonly ILoc _loc;

	private readonly Phrase _percentThresholdPhrase = Phrase.New("Automation.Threshold").FormatPercentRounded();

	private readonly Phrase _measurementPhrase = Phrase.New("Automation.Measurement").Format((int value) => value.ToString());

	private readonly Phrase _percentMeasurementPhrase = Phrase.New("Automation.Measurement").FormatPercentRounded();

	private VisualElement _root;

	private IntegerField _threshold;

	private Dropdown _goodDropdown;

	private RadioToggle _modeRadioToggle;

	private Toggle _includeInputsToggle;

	private Label _measurement;

	private Label _fillRateLabel;

	private ResourceCounter _resourceCounter;

	private ResourceCounterGoodsDropdownProvider _goodsDropdownProvider;

	private Dropdown _comparisonModeDropdown;

	private EnumDropdownProvider<NumericComparisonMode> _comparisonModeDropdownProvider;

	private PreciseSlider _fillRateSlider;

	public ResourceCounterFragment(VisualElementLoader visualElementLoader, NumericComparisonModeDropdownFactory numericComparisonModeDropdownFactory, DropdownItemsSetter dropdownItemsSetter, RadioToggleFactory radioToggleFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_numericComparisonModeDropdownFactory = numericComparisonModeDropdownFactory;
		_dropdownItemsSetter = dropdownItemsSetter;
		_radioToggleFactory = radioToggleFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ResourceCounterFragment");
		_modeRadioToggle = _radioToggleFactory.CreateLocalizable(new string[2] { PercentModeLocKey, QuantityModeLocKey }, _root.Q<VisualElement>("ModeRadioToggleContainer"));
		_root.ToggleDisplayStyle(visible: false);
		_threshold = _root.Q<IntegerField>("Threshold");
		_threshold.RegisterValueChangedCallback(ChangeThreshold);
		_threshold.isDelayed = true;
		_goodDropdown = _root.Q<Dropdown>("Good");
		_measurement = _root.Q<Label>("Measurement");
		_comparisonModeDropdown = _root.Q<Dropdown>("ComparisonMode");
		_comparisonModeDropdownProvider = _numericComparisonModeDropdownFactory.Create(() => _resourceCounter.ComparisonMode, delegate(NumericComparisonMode comparisonMode)
		{
			_resourceCounter.SetComparisonMode(comparisonMode);
		});
		_modeRadioToggle.RadioButtonSelected += OnModeChanged;
		_fillRateSlider = _root.Q<PreciseSlider>("FillRateSlider");
		_fillRateSlider.SetValueChangedCallback(SetFillRate);
		_fillRateSlider.SetStepWithoutNotify(PercentThresholdChangeStep);
		_fillRateLabel = _root.Q<Label>("FillRateLabel");
		_includeInputsToggle = _root.Q<Toggle>("Toggle");
		_includeInputsToggle.RegisterValueChangedCallback(ChangeIncludeInputs);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_resourceCounter = entity.GetComponent<ResourceCounter>();
		if ((bool)_resourceCounter)
		{
			_threshold.SetValueWithoutNotify(_resourceCounter.Threshold);
			_root.ToggleDisplayStyle(visible: true);
			_goodsDropdownProvider = _resourceCounter.GetComponent<ResourceCounterGoodsDropdownProvider>();
			_dropdownItemsSetter.SetItems(_goodDropdown, _goodsDropdownProvider);
			_dropdownItemsSetter.SetItems(_comparisonModeDropdown, _comparisonModeDropdownProvider);
			_fillRateSlider.UpdateValuesWithoutNotify(_resourceCounter.FillRateThreshold, 1f);
			_includeInputsToggle.SetValueWithoutNotify(_resourceCounter.IncludeInputs);
		}
	}

	public void ClearFragment()
	{
		_resourceCounter = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_resourceCounter)
		{
			_modeRadioToggle.Update((int)_resourceCounter.Mode);
			_fillRateSlider.SetMarker(_resourceCounter.SampledFillRate);
			Label measurement = _measurement;
			measurement.text = _resourceCounter.Mode switch
			{
				ResourceCounterMode.StockLevel => _loc.T(_measurementPhrase, _resourceCounter.SampledResourceCount), 
				ResourceCounterMode.FillRate => _loc.T(_percentMeasurementPhrase, _resourceCounter.SampledFillRate), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			_fillRateLabel.text = _loc.T(_percentThresholdPhrase, _resourceCounter.FillRateThreshold);
			_fillRateSlider.ToggleDisplayStyle(_resourceCounter.Mode == ResourceCounterMode.FillRate);
			_fillRateLabel.ToggleDisplayStyle(_resourceCounter.Mode == ResourceCounterMode.FillRate);
			_threshold.ToggleDisplayStyle(_resourceCounter.Mode == ResourceCounterMode.StockLevel);
			_includeInputsToggle.ToggleDisplayStyle(_resourceCounter.Mode == ResourceCounterMode.StockLevel);
		}
	}

	private void OnModeChanged(object sender, int intMode)
	{
		_resourceCounter.SetMode((ResourceCounterMode)intMode);
	}

	private void SetFillRate(float value)
	{
		_fillRateLabel.text = _loc.T(_percentThresholdPhrase, value);
		_resourceCounter.SetFillRateThreshold(value);
	}

	private void ChangeThreshold(ChangeEvent<int> evt)
	{
		int num = Math.Clamp(evt.newValue, 0, int.MaxValue);
		_threshold.SetValueWithoutNotify(num);
		_resourceCounter.SetThreshold(num);
	}

	private void ChangeIncludeInputs(ChangeEvent<bool> includeInputs)
	{
		_resourceCounter.SetIncludeInputs(includeInputs.newValue);
	}
}
