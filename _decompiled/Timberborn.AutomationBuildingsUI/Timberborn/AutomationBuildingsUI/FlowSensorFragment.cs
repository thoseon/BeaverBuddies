using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class FlowSensorFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly NumericComparisonModeDropdownFactory _numericComparisonModeDropdownFactory;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	private readonly Phrase _measurementPhrase;

	private readonly Phrase _thresholdPhrase;

	private FlowSensor _flowSensor;

	private VisualElement _root;

	private Label _measurement;

	private Label _thresholdLabel;

	private Dropdown _modeDropdown;

	private PreciseSlider _thresholdSlider;

	private EnumDropdownProvider<NumericComparisonMode> _modeDropdownProvider;

	public FlowSensorFragment(VisualElementLoader visualElementLoader, NumericComparisonModeDropdownFactory numericComparisonModeDropdownFactory, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_numericComparisonModeDropdownFactory = numericComparisonModeDropdownFactory;
		_dropdownItemsSetter = dropdownItemsSetter;
		_loc = loc;
		_measurementPhrase = Phrase.New("Automation.Measurement").FormatFlow<float>("F2");
		_thresholdPhrase = Phrase.New("Automation.Threshold").FormatFlow<float>("F2");
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WaterSensorFragment");
		_measurement = _root.Q<Label>("Measurement");
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_modeDropdownProvider = _numericComparisonModeDropdownFactory.Create(() => _flowSensor.Mode, delegate(NumericComparisonMode mode)
		{
			_flowSensor.SetMode(mode);
		});
		_thresholdLabel = _root.Q<Label>("ThresholdLabel");
		_thresholdSlider = _root.Q<PreciseSlider>("ThresholdSlider");
		_thresholdSlider.SetValueChangedCallback(OnThresholdChanged);
		_thresholdSlider.SetStepWithoutNotify(FlowSensor.Precision);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_flowSensor = entity.GetComponent<FlowSensor>();
		if ((bool)_flowSensor)
		{
			_dropdownItemsSetter.SetItems(_modeDropdown, _modeDropdownProvider);
			_thresholdSlider.UpdateValuesWithoutNotify(_flowSensor.Threshold, 0f, _flowSensor.MaxThreshold);
		}
	}

	public void ClearFragment()
	{
		_flowSensor = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle(_flowSensor);
		if ((bool)_flowSensor)
		{
			_measurement.text = (_flowSensor.SampledFlow.HasValue ? _loc.T(_measurementPhrase, _flowSensor.SampledFlow.Value) : "-");
			_thresholdLabel.text = _loc.T(_thresholdPhrase, _flowSensor.Threshold);
			if (_flowSensor.SampledFlow.HasValue)
			{
				_thresholdSlider.SetMarker(_flowSensor.SampledFlow.Value);
			}
			else
			{
				_thresholdSlider.ClearMarker();
			}
		}
	}

	private void OnThresholdChanged(float value)
	{
		_flowSensor.SetThreshold(value);
	}
}
