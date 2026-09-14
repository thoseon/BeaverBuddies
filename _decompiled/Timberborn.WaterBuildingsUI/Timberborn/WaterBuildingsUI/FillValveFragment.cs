using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WaterBuildings;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class FillValveFragment : IEntityPanelFragment
{
	private static readonly float TargetHeightStep = 0.05f;

	private static readonly string TargetHeightUnlimitedLocKey = "Building.FillValve.TargetHeightUnlimited";

	private static readonly string ActiveStateLabelClass = "entity-panel__text--highlight-white";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly Phrase _targetHeightPhrase = Phrase.New("Building.FillValve.TargetHeight").FormatDistance<float>("F2");

	private readonly Phrase _automationTargetHeightPhrase = Phrase.New("Building.FillValve.TargetHeight").FormatDistance<float>("F2");

	private FillValve _fillValve;

	private VisualElement _root;

	private Label _targetHeightLabel;

	private Label _targetHeightStateLabel;

	private PreciseSlider _targetHeightSlider;

	private Label _automationTargetHeightLabel;

	private Label _automationTargetHeightStateLabel;

	private VisualElement _automationTargetHeightWrapper;

	private PreciseSlider _automationTargetHeightSlider;

	private Toggle _synchronizeToggle;

	private float TargetHeightSliderMinValue => _fillValve.MinTargetHeight;

	private float TargetHeightSliderMaxValue => (float)_fillValve.MaxTargetHeight + TargetHeightStep;

	public FillValveFragment(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/FillValveFragment");
		_targetHeightLabel = _root.Q<Label>("TargetHeightLabel");
		_targetHeightStateLabel = _root.Q<Label>("TargetHeightStateLabel");
		_targetHeightSlider = _root.Q<PreciseSlider>("TargetHeightSlider");
		_targetHeightSlider.SetValueChangedCallback(SetTargetHeight);
		_automationTargetHeightWrapper = _root.Q<VisualElement>("AutomationTargetHeightWrapper");
		_automationTargetHeightLabel = _root.Q<Label>("AutomationTargetHeightLabel");
		_automationTargetHeightStateLabel = _root.Q<Label>("AutomationTargetHeightStateLabel");
		_automationTargetHeightSlider = _root.Q<PreciseSlider>("AutomationTargetHeightSlider");
		_automationTargetHeightSlider.SetValueChangedCallback(SetAutomationTargetHeight);
		_synchronizeToggle = _root.Q<Toggle>("Synchronize");
		_synchronizeToggle.RegisterValueChangedCallback(ToggleSynchronization);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_fillValve = entity.GetComponent<FillValve>();
		if ((bool)_fillValve)
		{
			_root.ToggleDisplayStyle(visible: true);
			_targetHeightSlider.SetStepWithoutNotify(TargetHeightStep);
			_automationTargetHeightSlider.SetStepWithoutNotify(TargetHeightStep);
		}
	}

	public void ClearFragment()
	{
		_fillValve = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_fillValve)
		{
			UpdateOutflowLimit();
			UpdateAutomationOutflowLimit();
			UpdateMarkers();
			UpdateSynchronizeToggle();
		}
	}

	private void UpdateOutflowLimit()
	{
		_targetHeightSlider.UpdateValuesWithoutNotify(_fillValve.TargetHeightEnabled ? Mathf.Clamp(_fillValve.ClampedTargetHeight, _fillValve.MinTargetHeight, _fillValve.MaxTargetHeight) : TargetHeightSliderMaxValue, TargetHeightSliderMinValue, TargetHeightSliderMaxValue);
		_targetHeightLabel.text = (_fillValve.TargetHeightEnabled ? _loc.T(_targetHeightPhrase, _fillValve.TargetDepth) : _loc.T(TargetHeightUnlimitedLocKey));
		_targetHeightStateLabel.ToggleDisplayStyle(_fillValve.IsAutomated);
		if (_fillValve.IsAutomated)
		{
			_targetHeightStateLabel.EnableInClassList(ActiveStateLabelClass, !_fillValve.IsInputOn);
		}
	}

	private void UpdateAutomationOutflowLimit()
	{
		_automationTargetHeightWrapper.ToggleDisplayStyle(_fillValve.IsAutomated);
		if (_fillValve.IsAutomated)
		{
			_automationTargetHeightSlider.UpdateValuesWithoutNotify(_fillValve.AutomationTargetHeightEnabled ? Mathf.Clamp(_fillValve.ClampedAutomationTargetHeight, _fillValve.MinTargetHeight, _fillValve.MaxTargetHeight) : TargetHeightSliderMaxValue, TargetHeightSliderMinValue, TargetHeightSliderMaxValue);
			_automationTargetHeightLabel.text = (_fillValve.AutomationTargetHeightEnabled ? _loc.T(_automationTargetHeightPhrase, _fillValve.AutomationTargetDepth) : _loc.T(TargetHeightUnlimitedLocKey));
			_automationTargetHeightStateLabel.EnableInClassList(ActiveStateLabelClass, _fillValve.IsInputOn);
		}
	}

	private void UpdateMarkers()
	{
		float actualHeight = _fillValve.ActualHeight;
		_targetHeightSlider.SetMarker(actualHeight);
		if (_fillValve.IsAutomated)
		{
			_automationTargetHeightSlider.SetMarker(actualHeight);
		}
	}

	private void UpdateSynchronizeToggle()
	{
		_synchronizeToggle.SetValueWithoutNotify(_fillValve.IsSynchronized);
	}

	private void SetTargetHeight(float value)
	{
		if (value > (float)_fillValve.MaxTargetHeight)
		{
			_fillValve.SetTargetHeightEnabledAndSynchronize(value: false);
			_fillValve.SetTargetHeightAndSynchronize(_fillValve.MaxTargetHeight);
		}
		else
		{
			_fillValve.SetTargetHeightEnabledAndSynchronize(value: true);
			_fillValve.SetTargetHeightAndSynchronize(value);
		}
	}

	private void SetAutomationTargetHeight(float value)
	{
		if (value > (float)_fillValve.MaxTargetHeight)
		{
			_fillValve.SetAutomationTargetHeightEnabledAndSynchronize(value: false);
			_fillValve.SetAutomationTargetHeightAndSynchronize(_fillValve.MaxTargetHeight);
		}
		else
		{
			_fillValve.SetAutomationTargetHeightEnabledAndSynchronize(value: true);
			_fillValve.SetAutomationTargetHeightAndSynchronize(value);
		}
	}

	private void ToggleSynchronization(ChangeEvent<bool> changeEvent)
	{
		_fillValve.ToggleSynchronization(changeEvent.newValue);
	}
}
