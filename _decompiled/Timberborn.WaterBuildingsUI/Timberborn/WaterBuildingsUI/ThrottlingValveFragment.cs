using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WaterBuildings;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class ThrottlingValveFragment : IEntityPanelFragment
{
	private static readonly string IdleLocKey = "Building.ThrottlingValve.State.Idle";

	private static readonly string OpeningLocKey = "Building.ThrottlingValve.State.Opening";

	private static readonly string ClosingLocKey = "Building.ThrottlingValve.State.Closing";

	private static readonly string OutflowUnlimitedLocKey = "Building.ThrottlingValve.OutflowUnlimited";

	private static readonly string ActiveStateLabelClass = "entity-panel__text--highlight-white";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly Phrase _outflowLimitPhrase = Phrase.New("Building.ThrottlingValve.OutflowLimit").FormatFlow<float>("F2");

	private readonly Phrase _automationOutflowLimitPhrase = Phrase.New("Building.ThrottlingValve.OutflowLimit").FormatFlow<float>("F2");

	private readonly Phrase _reactionSpeedPhrase = Phrase.New("Building.ThrottlingValve.ReactionSpeed").FormatPercentRounded();

	private ThrottlingValve _throttlingValve;

	private VisualElement _root;

	private Label _valveStateLabel;

	private Label _outflowLimitLabel;

	private Label _outflowLimitStateLabel;

	private PreciseSlider _outflowLimitSlider;

	private Label _automationOutflowLimitLabel;

	private Label _automationOutflowLimitStateLabel;

	private VisualElement _automationOutflowLimitWrapper;

	private PreciseSlider _automationOutflowLimitSlider;

	private VisualElement _reactionSpeedWrapper;

	private Label _reactionSpeedLabel;

	private PreciseSlider _reactionSpeedSlider;

	private Toggle _synchronizeToggle;

	private float OutflowLimitSliderMaxValue => _throttlingValve.MaxOutflowLimit + _throttlingValve.OutflowLimitStep;

	public ThrottlingValveFragment(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ThrottlingValveFragment");
		_valveStateLabel = _root.Q<Label>("ThrottlingValveState");
		_outflowLimitLabel = _root.Q<Label>("OutflowLimitLabel");
		_outflowLimitStateLabel = _root.Q<Label>("OutflowLimitStateLabel");
		_outflowLimitSlider = _root.Q<PreciseSlider>("OutflowLimitSlider");
		_outflowLimitSlider.SetValueChangedCallback(SetOutflowLimit);
		_automationOutflowLimitWrapper = _root.Q<VisualElement>("AutomationOutflowLimitWrapper");
		_automationOutflowLimitLabel = _root.Q<Label>("AutomationOutflowLimitLabel");
		_automationOutflowLimitStateLabel = _root.Q<Label>("AutomationOutflowLimitStateLabel");
		_automationOutflowLimitSlider = _root.Q<PreciseSlider>("AutomationOutflowLimitSlider");
		_automationOutflowLimitSlider.SetValueChangedCallback(SetAutomationOutflowLimit);
		_reactionSpeedWrapper = _root.Q<VisualElement>("ReactionSpeedWrapper");
		_reactionSpeedLabel = _root.Q<Label>("ReactionSpeedLabel");
		_reactionSpeedSlider = _root.Q<PreciseSlider>("ReactionSpeedSlider");
		_reactionSpeedSlider.SetValueChangedCallback(SetReactionSpeed);
		_synchronizeToggle = _root.Q<Toggle>("Synchronize");
		_synchronizeToggle.RegisterValueChangedCallback(ToggleSynchronization);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_throttlingValve = entity.GetComponent<ThrottlingValve>();
		if ((bool)_throttlingValve)
		{
			_outflowLimitSlider.SetStepWithoutNotify(_throttlingValve.OutflowLimitStep);
			_automationOutflowLimitSlider.SetStepWithoutNotify(_throttlingValve.OutflowLimitStep);
			_reactionSpeedSlider.SetStepWithoutNotify(_throttlingValve.ReactionSpeedStep);
		}
	}

	public void ClearFragment()
	{
		_throttlingValve = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_throttlingValve)
		{
			UpdateOutflowLimit();
			UpdateAutomationOutflowLimit();
			UpdateMarkers();
			UpdateReactionSpeed();
			UpdateValveState();
			UpdateSynchronizeToggle();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateOutflowLimit()
	{
		_outflowLimitSlider.UpdateValuesWithoutNotify(_throttlingValve.OutflowLimitEnabled ? Mathf.Clamp(_throttlingValve.OutflowLimit, 0f, _throttlingValve.MaxOutflowLimit) : OutflowLimitSliderMaxValue, OutflowLimitSliderMaxValue);
		_outflowLimitLabel.text = (_throttlingValve.OutflowLimitEnabled ? _loc.T(_outflowLimitPhrase, _throttlingValve.OutflowLimit) : _loc.T(OutflowUnlimitedLocKey));
		_outflowLimitStateLabel.ToggleDisplayStyle(_throttlingValve.IsAutomated);
		if (_throttlingValve.IsAutomated)
		{
			_outflowLimitStateLabel.EnableInClassList(ActiveStateLabelClass, !_throttlingValve.IsInputOn);
		}
	}

	private void UpdateAutomationOutflowLimit()
	{
		_automationOutflowLimitWrapper.ToggleDisplayStyle(_throttlingValve.IsAutomated);
		if (_throttlingValve.IsAutomated)
		{
			_automationOutflowLimitSlider.UpdateValuesWithoutNotify(_throttlingValve.AutomationOutflowLimitEnabled ? Mathf.Clamp(_throttlingValve.AutomationOutflowLimit, 0f, _throttlingValve.MaxOutflowLimit) : OutflowLimitSliderMaxValue, OutflowLimitSliderMaxValue);
			_automationOutflowLimitLabel.text = (_throttlingValve.AutomationOutflowLimitEnabled ? _loc.T(_automationOutflowLimitPhrase, _throttlingValve.AutomationOutflowLimit) : _loc.T(OutflowUnlimitedLocKey));
			_automationOutflowLimitStateLabel.EnableInClassList(ActiveStateLabelClass, _throttlingValve.IsInputOn);
		}
	}

	private void UpdateMarkers()
	{
		if (_throttlingValve.IsAutomated)
		{
			float marker = _throttlingValve.CurrentOutflowLimit ?? OutflowLimitSliderMaxValue;
			_outflowLimitSlider.SetMarker(marker);
			_automationOutflowLimitSlider.SetMarker(marker);
		}
		else
		{
			_outflowLimitSlider.ClearMarker();
			_automationOutflowLimitSlider.ClearMarker();
		}
	}

	private void UpdateReactionSpeed()
	{
		_reactionSpeedWrapper.ToggleDisplayStyle(_throttlingValve.IsAutomated);
		_reactionSpeedSlider.UpdateValuesWithoutNotify(_throttlingValve.ReactionSpeed, ThrottlingValve.ReactionSpeedMin, ThrottlingValve.ReactionSpeedMax);
		_reactionSpeedLabel.text = _loc.T(_reactionSpeedPhrase, _throttlingValve.ReactionSpeed);
	}

	private void UpdateValveState()
	{
		_valveStateLabel.ToggleDisplayStyle(_throttlingValve.State.HasValue);
		if (_throttlingValve.State.HasValue)
		{
			Label valveStateLabel = _valveStateLabel;
			valveStateLabel.text = _throttlingValve.State switch
			{
				ThrottlingValveState.Idle => _loc.T(IdleLocKey), 
				ThrottlingValveState.Opening => _loc.T(OpeningLocKey), 
				ThrottlingValveState.Closing => _loc.T(ClosingLocKey), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}

	private void UpdateSynchronizeToggle()
	{
		_synchronizeToggle.SetValueWithoutNotify(_throttlingValve.IsSynchronized);
	}

	private void SetOutflowLimit(float value)
	{
		if (value > _throttlingValve.MaxOutflowLimit)
		{
			_throttlingValve.SetOutflowLimitEnabledAndSynchronize(value: false);
			_throttlingValve.SetOutflowLimitAndSynchronize(_throttlingValve.MaxOutflowLimit);
		}
		else
		{
			_throttlingValve.SetOutflowLimitEnabledAndSynchronize(value: true);
			_throttlingValve.SetOutflowLimitAndSynchronize(value);
		}
	}

	private void SetAutomationOutflowLimit(float value)
	{
		if (value > _throttlingValve.MaxOutflowLimit)
		{
			_throttlingValve.SetAutomationOutflowLimitEnabledAndSynchronize(value: false);
			_throttlingValve.SetAutomationOutflowLimitAndSynchronize(_throttlingValve.MaxOutflowLimit);
		}
		else
		{
			_throttlingValve.SetAutomationOutflowLimitEnabledAndSynchronize(value: true);
			_throttlingValve.SetAutomationOutflowLimitAndSynchronize(value);
		}
	}

	private void SetReactionSpeed(float value)
	{
		_throttlingValve.SetReactionSpeedAndSynchronize(value);
	}

	private void ToggleSynchronization(ChangeEvent<bool> changeEvent)
	{
		_throttlingValve.ToggleSynchronization(changeEvent.newValue);
	}
}
