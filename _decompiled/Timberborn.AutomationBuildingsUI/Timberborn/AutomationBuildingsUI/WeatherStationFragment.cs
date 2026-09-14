using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class WeatherStationFragment : IEntityPanelFragment
{
	private static readonly string ModeLocKeyPrefix = "Weather.";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly RadioToggleFactory _radioToggleFactory;

	private readonly ILoc _loc;

	private readonly Phrase _earlyActivationPhrase = Phrase.New().FormatHours<int>();

	private VisualElement _root;

	private RadioToggle _modeRadioToggle;

	private Toggle _earlyActivationToggle;

	private VisualElement _earlyActivationWrapper;

	private Label _earlyActivationLabel;

	private PreciseSlider _earlyActivationSlider;

	private WeatherStation _weatherStation;

	public WeatherStationFragment(VisualElementLoader visualElementLoader, RadioToggleFactory radioToggleFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_radioToggleFactory = radioToggleFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WeatherStationFragment");
		_modeRadioToggle = _radioToggleFactory.CreateLocalizable<WeatherStationMode>(ModeLocKeyPrefix, _root.Q<VisualElement>("ModeRadioToggleContainer"));
		_modeRadioToggle.RadioButtonSelected += OnModeChanged;
		_earlyActivationToggle = _root.Q<Toggle>("EarlyActivationToggle");
		_earlyActivationToggle.RegisterValueChangedCallback(OnEarlyActivationToggleChanged);
		_earlyActivationWrapper = _root.Q("EarlyActivationWrapper");
		_earlyActivationLabel = _root.Q<Label>("EarlyActivationLabel");
		_earlyActivationSlider = _root.Q<PreciseSlider>("EarlyActivationSlider");
		_earlyActivationSlider.SetValueChangedCallback(OnEarlyActivationSliderChanged);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_weatherStation = entity.GetComponent<WeatherStation>();
	}

	public void ClearFragment()
	{
		_weatherStation = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_weatherStation)
		{
			_modeRadioToggle.Update((int)_weatherStation.Mode);
			_earlyActivationToggle.value = _weatherStation.EarlyActivationEnabled;
			_earlyActivationWrapper.ToggleDisplayStyle(_weatherStation.EarlyActivationEnabled);
			if (_weatherStation.EarlyActivationEnabled)
			{
				_earlyActivationLabel.text = _loc.T(_earlyActivationPhrase, _weatherStation.EarlyActivationHours);
				_earlyActivationSlider.UpdateValuesWithoutNotify(_weatherStation.EarlyActivationHours, _weatherStation.MaxEarlyActivationHours);
			}
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnModeChanged(object sender, int intMode)
	{
		_weatherStation.SetMode((WeatherStationMode)intMode);
	}

	private void OnEarlyActivationToggleChanged(ChangeEvent<bool> evt)
	{
		_weatherStation.SetEarlyActivationEnabled(evt.newValue);
	}

	private void OnEarlyActivationSliderChanged(float value)
	{
		_weatherStation.SetEarlyActivationHours(Mathf.RoundToInt(value));
	}
}
