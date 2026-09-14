using System;
using System.Collections.Generic;
using Timberborn.ActivatorSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.EntityUndoSystem;
using Timberborn.Localization;
using Timberborn.MapStateSystem;
using UnityEngine.UIElements;

namespace Timberborn.ActivatorSystemUI;

internal class TimedComponentActivatorSettingsFragment : IEntityPanelFragment
{
	private static readonly string CyclesUntilCountdownActivationLocKey = "TimedComponentActivator.CyclesUntilCountdownActivation";

	private static readonly string DaysUntilActivationLocKey = "TimedComponentActivator.DaysUntilActivationLoc";

	private static readonly float SettingsCyclesMinValue = 1f;

	private static readonly float DaysCyclesMinValue = 0f;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly TimedActivatorSettingFactory _timedActivatorSettingFactory;

	private readonly MapEditorMode _mapEditorMode;

	private readonly DevModeManager _devModeManager;

	private readonly EntityChangeRecorderFactory _entityChangeRecorderFactory;

	private VisualElement _root;

	private Toggle _isEnabledToggle;

	private VisualElement _settingsRoot;

	private TimedComponentActivator _timedComponentActivator;

	private readonly List<TimedActivatorSetting> _timedActivatorSettings = new List<TimedActivatorSetting>();

	public TimedComponentActivatorSettingsFragment(VisualElementLoader visualElementLoader, ILoc loc, TimedActivatorSettingFactory timedActivatorSettingFactory, MapEditorMode mapEditorMode, DevModeManager devModeManager, EntityChangeRecorderFactory entityChangeRecorderFactory)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_timedActivatorSettingFactory = timedActivatorSettingFactory;
		_mapEditorMode = mapEditorMode;
		_devModeManager = devModeManager;
		_entityChangeRecorderFactory = entityChangeRecorderFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/TimedComponentActivatorSettingsFragment");
		_settingsRoot = _root.Q<VisualElement>("TimedComponentActivatorSettings");
		_isEnabledToggle = _root.Q<Toggle>("IsEnabledToggle");
		_isEnabledToggle.RegisterValueChangedCallback(OnEnabledToggleStateChanged);
		AddSettings();
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_timedComponentActivator = entity.GetComponent<TimedComponentActivator>();
		if ((bool)(BaseComponent)(object)_timedComponentActivator && (_mapEditorMode.IsMapEditor || _devModeManager.Enabled))
		{
			UpdateSettings();
			_root.ToggleDisplayStyle(visible: true);
			_isEnabledToggle.ToggleDisplayStyle(_timedComponentActivator.IsOptional);
			_isEnabledToggle.SetValueWithoutNotify(_timedComponentActivator.IsEnabled);
			_settingsRoot.ToggleDisplayStyle(_timedComponentActivator.IsEnabled);
		}
	}

	public void ClearFragment()
	{
		_timedComponentActivator = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if (!(BaseComponent)(object)_timedComponentActivator)
		{
			return;
		}
		if (_devModeManager.Enabled || _mapEditorMode.IsMapEditor)
		{
			_root.ToggleDisplayStyle(visible: true);
			if (_isEnabledToggle.value != _timedComponentActivator.IsEnabled)
			{
				_isEnabledToggle.SetValueWithoutNotify(_timedComponentActivator.IsEnabled);
				_settingsRoot.ToggleDisplayStyle(_timedComponentActivator.IsEnabled);
			}
			UpdateSettings();
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnEnabledToggleStateChanged(ChangeEvent<bool> evt)
	{
		if (!(BaseComponent)(object)_timedComponentActivator || !_timedComponentActivator.IsOptional)
		{
			return;
		}
		using (_entityChangeRecorderFactory.CreateChangeRecorder((BaseComponent)(object)_timedComponentActivator))
		{
			if (evt.newValue)
			{
				_timedComponentActivator.EnableActivator();
			}
			else
			{
				_timedComponentActivator.DisableActivator();
			}
		}
		_settingsRoot.ToggleDisplayStyle(evt.newValue);
	}

	private void AddSettings()
	{
		AddSetting(_loc.T(CyclesUntilCountdownActivationLocKey), SetCyclesUntilCountdownActivation, () => _timedComponentActivator.CyclesUntilCountdownActivation, SettingsCyclesMinValue);
		AddSetting(_loc.T(DaysUntilActivationLocKey), SetDaysUntilActivation, () => _timedComponentActivator.DaysUntilActivation, DaysCyclesMinValue);
	}

	private void AddSetting(string label, Action<float> setter, Func<float> getter, float minValue)
	{
		TimedActivatorSetting timedActivatorSetting = _timedActivatorSettingFactory.Create(label, setter, getter, minValue);
		_timedActivatorSettings.Add(timedActivatorSetting);
		_settingsRoot.Add(timedActivatorSetting.Root);
	}

	private void UpdateSettings()
	{
		foreach (TimedActivatorSetting timedActivatorSetting in _timedActivatorSettings)
		{
			timedActivatorSetting.UpdateState();
		}
	}

	private void SetCyclesUntilCountdownActivation(float value)
	{
		using (_entityChangeRecorderFactory.CreateChangeRecorder((BaseComponent)(object)_timedComponentActivator))
		{
			_timedComponentActivator.SetCyclesUntilCountdownActivation((int)value);
		}
	}

	private void SetDaysUntilActivation(float value)
	{
		using (_entityChangeRecorderFactory.CreateChangeRecorder((BaseComponent)(object)_timedComponentActivator))
		{
			_timedComponentActivator.SetDaysUntilActivation(value);
		}
	}
}
