using System;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class ChronometerFragment : IEntityPanelFragment
{
	private static readonly string StartTimeLocKey = "Building.Chronometer.StartTime";

	private static readonly string EndTimeLocKey = "Building.Chronometer.EndTime";

	private static readonly string ModeLocKeyPrefix = "Building.Chronometer.Mode.";

	private static readonly float TimeChangeStep = 0.25f;

	private readonly Phrase _hoursShortPhrase = Phrase.New().FormatHours<float>("F2");

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly RadioToggleFactory _radioToggleFactory;

	private VisualElement _root;

	private RadioToggle _modeRadioToggle;

	private VisualElement _timeRangeControls;

	private Label _startLabel;

	private Label _endLabel;

	private PreciseSlider _startSlider;

	private PreciseSlider _endSlider;

	private Chronometer _chronometer;

	public ChronometerFragment(VisualElementLoader visualElementLoader, ILoc loc, RadioToggleFactory radioToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_radioToggleFactory = radioToggleFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ChronometerFragment");
		_modeRadioToggle = _radioToggleFactory.CreateLocalizable<ChronometerMode>(ModeLocKeyPrefix, _root.Q<VisualElement>("Mode"));
		_modeRadioToggle.RadioButtonSelected += OnModeChanged;
		_timeRangeControls = _root.Q<VisualElement>("TimeRangeControls");
		_startLabel = _root.Q<Label>("StartLabel");
		_endLabel = _root.Q<Label>("EndLabel");
		_startSlider = _root.Q<PreciseSlider>("StartSlider");
		_startSlider.SetValueChangedCallback(SetStartTime);
		_startSlider.SetStepWithoutNotify(TimeChangeStep);
		_endSlider = _root.Q<PreciseSlider>("EndSlider");
		_endSlider.SetValueChangedCallback(SetEndTime);
		_endSlider.SetStepWithoutNotify(TimeChangeStep);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<Chronometer>(out _chronometer))
		{
			_root.ToggleDisplayStyle(visible: true);
			_startSlider.UpdateValuesWithoutNotify(_chronometer.StartTime, 24f);
			UpdateStartLabel();
			_endSlider.UpdateValuesWithoutNotify(_chronometer.EndTime, 24f);
			UpdateEndLabel();
		}
	}

	public void ClearFragment()
	{
		_chronometer = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if (_chronometer != null)
		{
			_modeRadioToggle.Update((int)_chronometer.Mode);
			_timeRangeControls.ToggleDisplayStyle(_chronometer.Mode == ChronometerMode.TimeRange);
			_startSlider.SetMarker(_chronometer.SampledTime);
			_endSlider.SetMarker(_chronometer.SampledTime);
		}
	}

	private void OnModeChanged(object sender, int index)
	{
		_chronometer.SetMode((ChronometerMode)index);
	}

	private void SetStartTime(float value)
	{
		float startTime = ClampTime(value);
		_chronometer.SetStartTime(startTime);
		UpdateStartLabel();
	}

	private void UpdateStartLabel()
	{
		_startLabel.text = _loc.T(StartTimeLocKey, GetHoursText(_chronometer.StartTime));
	}

	private void SetEndTime(float value)
	{
		float endTime = ClampTime(value);
		_chronometer.SetEndTime(endTime);
		UpdateEndLabel();
	}

	private void UpdateEndLabel()
	{
		_endLabel.text = _loc.T(EndTimeLocKey, GetHoursText(_chronometer.EndTime));
	}

	private string GetHoursText(float value)
	{
		return _loc.T(_hoursShortPhrase, value);
	}

	private static float ClampTime(float time)
	{
		return Math.Clamp(time, 0f, 24f);
	}
}
