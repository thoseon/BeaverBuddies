using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class DepthSensorFragment : IEntityPanelFragment
{
	private static readonly float ThresholdChangeStep = 0.05f;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly NumericComparisonModeDropdownFactory _numericComparisonModeDropdownFactory;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	private DepthSensor _depthSensor;

	private VisualElement _root;

	private Label _measurement;

	private Label _thresholdLabel;

	private Dropdown _modeDropdown;

	private PreciseSlider _thresholdSlider;

	private EnumDropdownProvider<NumericComparisonMode> _modeDropdownProvider;

	private readonly Phrase _measurementPhrase = Phrase.New("Automation.Measurement").FormatDistance<float>("F2");

	private readonly Phrase _thresholdPhrase = Phrase.New("Automation.Threshold").FormatDistance<float>("F2");

	public DepthSensorFragment(VisualElementLoader visualElementLoader, NumericComparisonModeDropdownFactory numericComparisonModeDropdownFactory, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_numericComparisonModeDropdownFactory = numericComparisonModeDropdownFactory;
		_dropdownItemsSetter = dropdownItemsSetter;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WaterSensorFragment");
		_measurement = _root.Q<Label>("Measurement");
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_modeDropdownProvider = _numericComparisonModeDropdownFactory.Create(() => _depthSensor.Mode, delegate(NumericComparisonMode mode)
		{
			_depthSensor.SetMode(mode);
		});
		_thresholdLabel = _root.Q<Label>("ThresholdLabel");
		_thresholdSlider = _root.Q<PreciseSlider>("ThresholdSlider");
		_thresholdSlider.SetValueChangedCallback(OnThresholdChanged);
		_thresholdSlider.SetStepWithoutNotify(ThresholdChangeStep);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_depthSensor = entity.GetComponent<DepthSensor>();
		if ((bool)_depthSensor)
		{
			_dropdownItemsSetter.SetItems(_modeDropdown, _modeDropdownProvider);
		}
	}

	public void ClearFragment()
	{
		_depthSensor = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle(_depthSensor);
		if ((bool)_depthSensor)
		{
			_measurement.text = _loc.T(_measurementPhrase, _depthSensor.DepthFromFloor);
			_thresholdLabel.text = _loc.T(_thresholdPhrase, _depthSensor.ThresholdFromFloor);
			_thresholdSlider.UpdateValuesWithoutNotify(_depthSensor.Threshold, _depthSensor.MinThreshold, _depthSensor.MaxThreshold);
			_thresholdSlider.SetMarker(_depthSensor.Depth);
		}
	}

	private void OnThresholdChanged(float value)
	{
		_depthSensor.SetThreshold(value);
	}
}
