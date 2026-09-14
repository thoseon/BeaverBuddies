using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class ContaminationSensorFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly NumericComparisonModeDropdownFactory _numericComparisonModeDropdownFactory;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	private ContaminationSensor _contaminationSensor;

	private VisualElement _root;

	private Label _measurement;

	private Label _thresholdLabel;

	private Dropdown _modeDropdown;

	private PreciseSlider _thresholdSlider;

	private EnumDropdownProvider<NumericComparisonMode> _modeDropdownProvider;

	private readonly Phrase _measurementPhrase = Phrase.New("Automation.Measurement").FormatPercentRounded();

	private readonly Phrase _thresholdPhrase = Phrase.New("Automation.Threshold").FormatPercentRounded();

	public ContaminationSensorFragment(VisualElementLoader visualElementLoader, NumericComparisonModeDropdownFactory numericComparisonModeDropdownFactory, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
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
		_modeDropdownProvider = _numericComparisonModeDropdownFactory.Create(() => _contaminationSensor.Mode, delegate(NumericComparisonMode mode)
		{
			_contaminationSensor.SetMode(mode);
		});
		_thresholdLabel = _root.Q<Label>("ThresholdLabel");
		_thresholdSlider = _root.Q<PreciseSlider>("ThresholdSlider");
		_thresholdSlider.SetValueChangedCallback(OnThresholdChanged);
		_thresholdSlider.SetStepWithoutNotify(ContaminationSensor.Precision);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_contaminationSensor = entity.GetComponent<ContaminationSensor>();
		if ((bool)_contaminationSensor)
		{
			_dropdownItemsSetter.SetItems(_modeDropdown, _modeDropdownProvider);
			_thresholdSlider.UpdateValuesWithoutNotify(_contaminationSensor.Threshold, 1f);
		}
	}

	public void ClearFragment()
	{
		_contaminationSensor = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle(_contaminationSensor);
		if ((bool)_contaminationSensor)
		{
			float? sampledContamination = _contaminationSensor.SampledContamination;
			_measurement.text = (sampledContamination.HasValue ? _loc.T(_measurementPhrase, sampledContamination.Value) : "-");
			_thresholdLabel.text = _loc.T(_thresholdPhrase, _contaminationSensor.Threshold);
			if (_contaminationSensor.SampledContamination.HasValue)
			{
				_thresholdSlider.SetMarker(_contaminationSensor.SampledContamination.Value);
			}
			else
			{
				_thresholdSlider.ClearMarker();
			}
		}
	}

	private void OnThresholdChanged(float value)
	{
		_contaminationSensor.SetThreshold(value);
	}
}
