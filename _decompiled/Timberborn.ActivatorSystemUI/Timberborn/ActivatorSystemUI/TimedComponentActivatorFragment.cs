using Timberborn.ActivatorSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.MapStateSystem;
using UnityEngine.UIElements;

namespace Timberborn.ActivatorSystemUI;

internal class TimedComponentActivatorFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly TimedActivatorProgressBarFactory _progressBarFactory;

	private readonly MapEditorMode _mapEditorMode;

	private VisualElement _root;

	private TimedActivatorProgressBar _progressBar;

	private TimedComponentActivator _timedComponentActivator;

	private ActivationWarningStatus _activationWarningStatus;

	public TimedComponentActivatorFragment(VisualElementLoader visualElementLoader, TimedActivatorProgressBarFactory progressBarFactory, MapEditorMode mapEditorMode)
	{
		_visualElementLoader = visualElementLoader;
		_progressBarFactory = progressBarFactory;
		_mapEditorMode = mapEditorMode;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/TimedComponentActivatorFragment");
		_progressBar = _progressBarFactory.Create(_root, GetActivationProgress, GetDaysLeftUntilActivation, CountdownIsActive);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			TimedComponentActivator component = entity.GetComponent<TimedComponentActivator>();
			if ((bool)(BaseComponent)(object)component && component.IsEnabled)
			{
				_timedComponentActivator = component;
				TimedComponentActivatorSpec component2 = ((BaseComponent)(object)_timedComponentActivator).GetComponent<TimedComponentActivatorSpec>();
				_progressBar.Initialize(component2.ProgressBarActiveLabelLocKey, component2.ProgressBarNotActiveLabelLocKey, component2.IsHazardousActivator);
				_activationWarningStatus = entity.GetComponent<ActivationWarningStatus>();
				_root.ToggleDisplayStyle(visible: true);
			}
		}
	}

	public void ClearFragment()
	{
		_timedComponentActivator = null;
		_activationWarningStatus = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_timedComponentActivator)
		{
			if (_timedComponentActivator.IsPastActivationTime)
			{
				_root.ToggleDisplayStyle(visible: false);
			}
			else
			{
				_progressBar.UpdateState();
			}
		}
	}

	private float GetActivationProgress()
	{
		return _timedComponentActivator.ActivationProgress;
	}

	private string GetDaysLeftUntilActivation()
	{
		string text = (_activationWarningStatus.IsCloseToActivation() ? "F1" : "F0");
		return _activationWarningStatus.GetDaysLeftUntilActivation().ToString(text);
	}

	private bool CountdownIsActive()
	{
		return _timedComponentActivator.CountdownIsActive;
	}
}
