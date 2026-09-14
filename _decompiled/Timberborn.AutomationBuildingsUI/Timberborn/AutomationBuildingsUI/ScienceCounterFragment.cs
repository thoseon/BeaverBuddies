using System;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class ScienceCounterFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly NumericComparisonModeDropdownFactory _numericComparisonModeDropdownFactory;

	private readonly ILoc _loc;

	private VisualElement _root;

	private IntegerField _threshold;

	private ScienceCounter _scienceCounter;

	private Label _measurement;

	private Dropdown _modeDropdown;

	private EnumDropdownProvider<NumericComparisonMode> _modeDropdownProvider;

	private readonly Phrase _measurementPhrase = Phrase.New("Automation.Measurement").Format((int value) => $"{value}");

	public ScienceCounterFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter, NumericComparisonModeDropdownFactory numericComparisonModeDropdownFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
		_numericComparisonModeDropdownFactory = numericComparisonModeDropdownFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ScienceCounterFragment");
		_root.ToggleDisplayStyle(visible: false);
		_measurement = _root.Q<Label>("Measurement");
		_threshold = _root.Q<IntegerField>("Threshold");
		_threshold.RegisterValueChangedCallback(ChangeThreshold);
		_threshold.isDelayed = true;
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_modeDropdownProvider = _numericComparisonModeDropdownFactory.Create(() => _scienceCounter.Mode, delegate(NumericComparisonMode mode)
		{
			_scienceCounter.SetMode(mode);
		});
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_scienceCounter = entity.GetComponent<ScienceCounter>();
		if ((bool)_scienceCounter)
		{
			_threshold.SetValueWithoutNotify(_scienceCounter.Threshold);
			_root.ToggleDisplayStyle(visible: true);
			_dropdownItemsSetter.SetItems(_modeDropdown, _modeDropdownProvider);
		}
	}

	public void ClearFragment()
	{
		_scienceCounter = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_scienceCounter)
		{
			_measurement.text = _loc.T(_measurementPhrase, _scienceCounter.SampledSciencePoints);
		}
	}

	private void ChangeThreshold(ChangeEvent<int> evt)
	{
		int num = Math.Clamp(evt.newValue, 0, int.MaxValue);
		_scienceCounter.SetThreshold(num);
		_threshold.SetValueWithoutNotify(num);
	}
}
