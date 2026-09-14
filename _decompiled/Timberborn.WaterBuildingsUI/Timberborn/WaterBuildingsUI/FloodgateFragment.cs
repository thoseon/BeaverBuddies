using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.UIFormatters;
using Timberborn.WaterBuildings;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class FloodgateFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly float ChangeTimeThreshold = 0.1f;

	private static readonly float HeightChangeStep = 0.05f;

	private static readonly string DecreaseFloodgateHeightKey = "DecreaseFloodgateHeight";

	private static readonly string IncreaseFloodgateHeightKey = "IncreaseFloodgateHeight";

	private static readonly string ActiveStateLabelClass = "entity-panel__text--highlight-white";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly EventBus _eventBus;

	private readonly InputService _inputService;

	private readonly Phrase _heightPhrase = Phrase.New("Building.Floodgate.Height").FormatDistance<float>("F2");

	private VisualElement _root;

	private Label _height;

	private Label _heightState;

	private Slider _heightSlider;

	private VisualElement _automationHeightWrapper;

	private Label _automationHeight;

	private Label _automationHeightState;

	private Slider _automationHeightSlider;

	private Toggle _synchronizeToggle;

	private Floodgate _floodgate;

	private float _timeSinceLastChange;

	private bool _heightChangedOnHold;

	public FloodgateFragment(VisualElementLoader visualElementLoader, ILoc loc, EventBus eventBus, InputService inputService)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_eventBus = eventBus;
		_inputService = inputService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/FloodgateFragment");
		_height = _root.Q<Label>("Height");
		_heightState = _root.Q<Label>("HeightState");
		_heightSlider = _root.Q<Slider>("HeightSlider");
		_automationHeightWrapper = _root.Q<VisualElement>("AutomationHeightWrapper");
		_automationHeight = _root.Q<Label>("AutomationHeight");
		_automationHeightState = _root.Q<Label>("AutomationHeightState");
		_automationHeightSlider = _root.Q<Slider>("AutomationHeightSlider");
		_synchronizeToggle = _root.Q<Toggle>("Synchronize");
		_heightSlider.RegisterValueChangedCallback(OnHeightSliderValueChanged);
		_automationHeightSlider.RegisterValueChangedCallback(OnAutomationHeightSliderValueChanged);
		_synchronizeToggle.RegisterValueChangedCallback(ToggleSynchronization);
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		Floodgate component = entity.GetComponent<Floodgate>();
		if ((bool)component)
		{
			_heightSlider.highValue = component.MaxHeight;
			_automationHeightSlider.highValue = component.MaxHeight;
			_heightSlider.SetValueWithoutNotify(component.Height);
			_automationHeightSlider.SetValueWithoutNotify(component.AutomationHeight);
			_inputService.AddInputProcessor(this);
		}
		_floodgate = component;
	}

	public void ClearFragment()
	{
		_floodgate = null;
		_root.ToggleDisplayStyle(visible: false);
		_inputService.RemoveInputProcessor(this);
	}

	public void UpdateFragment()
	{
		if ((bool)_floodgate)
		{
			_height.text = _loc.T(_heightPhrase, _floodgate.Height);
			_heightState.ToggleDisplayStyle(_floodgate.IsAutomated);
			_automationHeight.text = _loc.T(_heightPhrase, _floodgate.AutomationHeight);
			_synchronizeToggle.SetValueWithoutNotify(_floodgate.IsSynchronized);
			_root.ToggleDisplayStyle(visible: true);
			_automationHeightWrapper.ToggleDisplayStyle(_floodgate.IsAutomated);
			_heightState.EnableInClassList(ActiveStateLabelClass, !_floodgate.IsInputOn);
			_automationHeightState.EnableInClassList(ActiveStateLabelClass, _floodgate.IsInputOn);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyHeld(DecreaseFloodgateHeightKey))
		{
			DecreaseHeight();
			return true;
		}
		if (_inputService.IsKeyUp(DecreaseFloodgateHeightKey) && !_heightChangedOnHold)
		{
			DecreaseHeightIfPossible();
			return true;
		}
		if (_inputService.IsKeyHeld(IncreaseFloodgateHeightKey))
		{
			IncreaseHeight();
			return true;
		}
		if (_inputService.IsKeyUp(IncreaseFloodgateHeightKey) && !_heightChangedOnHold)
		{
			IncreaseHeightIfPossible();
			return true;
		}
		_timeSinceLastChange = 0f;
		_heightChangedOnHold = false;
		return false;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		GameObject gameObject = enteredFinishedStateEvent.BlockObject.GameObject;
		if ((bool)_floodgate && _floodgate.GameObject == gameObject)
		{
			_heightSlider.SetValueWithoutNotify(_floodgate.Height);
			_automationHeightSlider.SetValueWithoutNotify(_floodgate.AutomationHeight);
		}
	}

	private void OnHeightSliderValueChanged(ChangeEvent<float> evt)
	{
		ChangeHeight(evt.newValue);
	}

	private void OnAutomationHeightSliderValueChanged(ChangeEvent<float> evt)
	{
		ChangeAutomationHeight(evt.newValue);
	}

	private void DecreaseHeight()
	{
		if (_timeSinceLastChange > ChangeTimeThreshold)
		{
			DecreaseHeightIfPossible();
			_heightChangedOnHold = true;
			_timeSinceLastChange = 0f;
		}
		_timeSinceLastChange += Time.unscaledDeltaTime;
	}

	private void IncreaseHeight()
	{
		if (_timeSinceLastChange > ChangeTimeThreshold)
		{
			IncreaseHeightIfPossible();
			_heightChangedOnHold = true;
			_timeSinceLastChange = 0f;
		}
		_timeSinceLastChange += Time.unscaledDeltaTime;
	}

	private void DecreaseHeightIfPossible()
	{
		if (_floodgate.Height > 0f)
		{
			ChangeHeight(_floodgate.Height - HeightChangeStep);
		}
	}

	private void IncreaseHeightIfPossible()
	{
		if (_floodgate.Height < (float)_floodgate.MaxHeight)
		{
			ChangeHeight(_floodgate.Height + HeightChangeStep);
		}
	}

	private void ChangeHeight(float newHeight)
	{
		float num = UpdateHeightSliderValue(newHeight);
		if ((bool)_floodgate && _floodgate.Height != num)
		{
			_floodgate.SetHeightAndSynchronize(num);
		}
	}

	private void ChangeAutomationHeight(float newHeight)
	{
		float num = UpdateAutomationHeightSliderValue(newHeight);
		if ((bool)_floodgate && _floodgate.AutomationHeight != num)
		{
			_floodgate.SetAutomationHeightAndSynchronize(num);
		}
	}

	private float UpdateHeightSliderValue(float value)
	{
		float num = RoundHeight(value);
		_heightSlider.SetValueWithoutNotify(num);
		return num;
	}

	private float UpdateAutomationHeightSliderValue(float value)
	{
		float num = RoundHeight(value);
		_automationHeightSlider.SetValueWithoutNotify(num);
		return num;
	}

	private void ToggleSynchronization(ChangeEvent<bool> changeEvent)
	{
		_floodgate.ToggleSynchronization(changeEvent.newValue);
		UpdateHeightSliderValue(_floodgate.Height);
		UpdateAutomationHeightSliderValue(_floodgate.AutomationHeight);
	}

	private static float RoundHeight(float value)
	{
		return (float)Math.Round(value * 20f) / 20f;
	}
}
