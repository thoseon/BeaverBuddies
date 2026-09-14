using System;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class PopulationCounterFragment : IEntityPanelFragment
{
	private static readonly string MeasurementLocKey = "Automation.Measurement";

	private static readonly string PopulationCounterModeLocKeyPrefix = "Building.PopulationCounter.Mode.";

	private static readonly string GlobalModeLocKey = "Building.PopulationCounter.GlobalMode";

	private static readonly string DistrictModeLocKey = "Building.PopulationCounter.DistrictMode";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly NumericComparisonModeDropdownFactory _numericComparisonModeDropdownFactory;

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly RadioToggleFactory _radioToggleFactory;

	private readonly ILoc _loc;

	private VisualElement _root;

	private RadioToggle _globalModeRadioToggle;

	private IntegerField _threshold;

	private Label _measurement;

	private PopulationCounter _populationCounter;

	private Dropdown _comparisonModeDropdown;

	private EnumDropdownProvider<NumericComparisonMode> _comparisonModeDropdownProvider;

	private Dropdown _modeDropdown;

	private EnumDropdownProvider<PopulationCounterMode> _populationCounterModeDropdownProvider;

	private VisualElement _workerTypeWrapper;

	private Toggle _beaversToggle;

	private Toggle _botsToggle;

	public PopulationCounterFragment(VisualElementLoader visualElementLoader, NumericComparisonModeDropdownFactory numericComparisonModeDropdownFactory, EnumDropdownProviderFactory enumDropdownProviderFactory, DropdownItemsSetter dropdownItemsSetter, RadioToggleFactory radioToggleFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_numericComparisonModeDropdownFactory = numericComparisonModeDropdownFactory;
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_dropdownItemsSetter = dropdownItemsSetter;
		_radioToggleFactory = radioToggleFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/PopulationCounterFragment");
		_root.ToggleDisplayStyle(visible: false);
		_globalModeRadioToggle = _radioToggleFactory.CreateLocalizable(new string[2] { DistrictModeLocKey, GlobalModeLocKey }, _root.Q<VisualElement>("GlobalModeRadioToggle"));
		_globalModeRadioToggle.RadioButtonSelected += OnGlobalModeChanged;
		_threshold = _root.Q<IntegerField>("Threshold");
		_threshold.RegisterValueChangedCallback(OnThresholdChanged);
		_threshold.isDelayed = true;
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_populationCounterModeDropdownProvider = _enumDropdownProviderFactory.CreateLocalized(() => _populationCounter.Mode, SetMode, PopulationCounterModeLocKeyPrefix);
		_workerTypeWrapper = _root.Q<VisualElement>("WorkerTypeWrapper");
		_beaversToggle = _root.Q<Toggle>("BeaversToggle");
		_botsToggle = _root.Q<Toggle>("BotsToggle");
		_beaversToggle.RegisterValueChangedCallback(OnBeaversToggleChanged);
		_botsToggle.RegisterValueChangedCallback(OnBotsToggleChanged);
		_measurement = _root.Q<Label>("Measurement");
		_comparisonModeDropdown = _root.Q<Dropdown>("ComparisonMode");
		_comparisonModeDropdownProvider = _numericComparisonModeDropdownFactory.Create(() => _populationCounter.ComparisonMode, delegate(NumericComparisonMode comparisonMode)
		{
			_populationCounter.SetComparisonMode(comparisonMode);
		});
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_populationCounter = entity.GetComponent<PopulationCounter>();
		if ((bool)_populationCounter)
		{
			_threshold.SetValueWithoutNotify(_populationCounter.Threshold);
			_beaversToggle.SetValueWithoutNotify(_populationCounter.CountBeavers);
			_botsToggle.SetValueWithoutNotify(_populationCounter.CountBots);
			_root.ToggleDisplayStyle(visible: true);
			_dropdownItemsSetter.SetItems(_modeDropdown, _populationCounterModeDropdownProvider);
			_dropdownItemsSetter.SetItems(_comparisonModeDropdown, _comparisonModeDropdownProvider);
		}
	}

	public void ClearFragment()
	{
		_populationCounter = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_populationCounter)
		{
			_globalModeRadioToggle.Update(_populationCounter.GlobalMode ? 1 : 0);
			_workerTypeWrapper.ToggleDisplayStyle(_populationCounter.UsesWorkerType);
			_measurement.text = _loc.T(MeasurementLocKey, _populationCounter.GetMeasurement());
		}
	}

	private void OnGlobalModeChanged(object sender, int modeIndex)
	{
		_populationCounter.SetGlobalMode(modeIndex == 1);
	}

	private void SetMode(PopulationCounterMode mode)
	{
		_populationCounter.SetMode(mode);
	}

	private void OnBeaversToggleChanged(ChangeEvent<bool> evt)
	{
		_populationCounter.SetCountBeavers(evt.newValue);
	}

	private void OnBotsToggleChanged(ChangeEvent<bool> evt)
	{
		_populationCounter.SetCountBots(evt.newValue);
	}

	private void OnThresholdChanged(ChangeEvent<int> evt)
	{
		int num = Math.Clamp(evt.newValue, 0, int.MaxValue);
		_threshold.SetValueWithoutNotify(num);
		_populationCounter.SetThreshold(num);
	}
}
