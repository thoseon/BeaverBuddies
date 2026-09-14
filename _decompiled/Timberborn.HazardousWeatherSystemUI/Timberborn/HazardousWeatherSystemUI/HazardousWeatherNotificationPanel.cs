using Timberborn.BlueprintSystem;
using Timberborn.CameraSystem;
using Timberborn.CoreUI;
using Timberborn.GameCycleSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using Timberborn.WeatherSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.HazardousWeatherSystemUI;

internal class HazardousWeatherNotificationPanel : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly float CameraShiftScale = -650f;

	private static readonly string FadeClass = "hazardous-weather-notification__fade--enabled";

	private static readonly string WetWeatherClass = "hazardous-weather-notification__background--wet";

	private static readonly string CycleBeginsKey = "Weather.CycleBegins";

	private readonly ILoc _loc;

	private readonly EventBus _eventBus;

	private readonly HazardousWeatherUIHelper _hazardousWeatherUIHelper;

	private readonly UILayout _uiLayout;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WeatherService _weatherService;

	private readonly CameraHorizontalShifter _cameraHorizontalShifter;

	private readonly ISpecService _specService;

	private HazardousWeatherUISpec _spec;

	private VisualElement _panel;

	private Image _background;

	private Label _header;

	private Label _description;

	private float _notificationTimer;

	private bool _showApproachingNotificationIfUnpaused;

	private bool _isTimerBlockerActive;

	public HazardousWeatherNotificationPanel(ILoc loc, EventBus eventBus, HazardousWeatherUIHelper hazardousWeatherUIHelper, UILayout uiLayout, PanelStack panelStack, VisualElementLoader visualElementLoader, WeatherService weatherService, CameraHorizontalShifter cameraHorizontalShifter, ISpecService specService)
	{
		_loc = loc;
		_eventBus = eventBus;
		_hazardousWeatherUIHelper = hazardousWeatherUIHelper;
		_uiLayout = uiLayout;
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_weatherService = weatherService;
		_cameraHorizontalShifter = cameraHorizontalShifter;
		_specService = specService;
	}

	public void Load()
	{
		_spec = _specService.GetSingleSpec<HazardousWeatherUISpec>();
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/HazardousWeatherNotificationPanel");
		_panel = visualElement.Q<VisualElement>("HazardousWeatherNotificationPanel");
		_panel.ToggleDisplayStyle(visible: false);
		_panel.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);
		_panel.RegisterCallback<TransitionCancelEvent>(OnTransitionCancel);
		_header = visualElement.Q<Label>("Header");
		_description = visualElement.Q<Label>("Description");
		_background = visualElement.Q<Image>("Background");
		_uiLayout.AddAbsoluteItem(visualElement);
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_panel.IsDisplayed())
		{
			UpdateTimer();
			UpdatePanelPosition();
		}
		if (_showApproachingNotificationIfUnpaused && Time.timeScale != 0f)
		{
			_showApproachingNotificationIfUnpaused = false;
			ShowHazardousSeasonNotification(_loc.T(_hazardousWeatherUIHelper.ApproachingLocKey));
		}
	}

	[OnEvent]
	public void OnHazardousWeatherStarted(HazardousWeatherStartedEvent hazardousWeatherStartedEvent)
	{
		ShowHazardousSeasonNotification(_loc.T(_hazardousWeatherUIHelper.StartedNotificationLocKey));
	}

	[OnEvent]
	public void OnCycleEndedEvent(CycleEndedEvent cycleEndedEvent)
	{
		ShowTemperateSeasonNotification(cycleEndedEvent.Cycle + 1);
	}

	[OnEvent]
	public void OnHazardousWeatherApproaching(HazardousWeatherApproachingEvent hazardousWeatherApproachingEvent)
	{
		_showApproachingNotificationIfUnpaused = true;
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		UpdateTimerBlocker();
	}

	[OnEvent]
	public void OnPanelHidden(PanelHiddenEvent panelHiddenEvent)
	{
		UpdateTimerBlocker();
	}

	private void OnTransitionEnd(TransitionEndEvent evt)
	{
		if (_panel.style.opacity == 0f)
		{
			_panel.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnTransitionCancel(TransitionCancelEvent evt)
	{
		_panel.ToggleDisplayStyle(visible: false);
	}

	private void UpdateTimerBlocker()
	{
		_isTimerBlockerActive = _panelStack.ContainsPanelBlocker();
	}

	private void UpdateTimer()
	{
		if (!_isTimerBlockerActive)
		{
			_notificationTimer += Time.unscaledDeltaTime;
			if (_notificationTimer > _spec.NotificationDuration)
			{
				SetPanelFade(fadeEnabled: false);
			}
		}
	}

	private void UpdatePanelPosition()
	{
		_panel.style.translate = new Translate(_cameraHorizontalShifter.CurrentOffset * CameraShiftScale, 0f);
	}

	private void ShowHazardousSeasonNotification(string seasonText)
	{
		ShowNotification(seasonText, null, isHazardous: true);
	}

	private void ShowTemperateSeasonNotification(int beginningCycle)
	{
		string text = _loc.T(CycleBeginsKey, beginningCycle);
		if (_weatherService.HazardousWeatherDuration > 0)
		{
			ShowNotification(_loc.T(_hazardousWeatherUIHelper.EndedNotificationLocKey), text, isHazardous: false);
		}
		else
		{
			ShowNotification(text, null, isHazardous: false);
		}
	}

	private void ShowNotification(string headerText, string descriptionText, bool isHazardous)
	{
		_header.text = headerText;
		_description.text = descriptionText;
		_description.ToggleDisplayStyle(!string.IsNullOrEmpty(descriptionText));
		if (isHazardous)
		{
			_background.AddToClassList(_hazardousWeatherUIHelper.NotificationBackgroundClass);
			_background.RemoveFromClassList(WetWeatherClass);
		}
		else
		{
			_background.RemoveFromClassList(_hazardousWeatherUIHelper.NotificationBackgroundClass);
			_background.AddToClassList(WetWeatherClass);
		}
		_panel.ToggleDisplayStyle(visible: true);
		_notificationTimer = 0f;
		SetPanelFade(fadeEnabled: true);
	}

	private void SetPanelFade(bool fadeEnabled)
	{
		_panel.EnableInClassList(FadeClass, fadeEnabled);
	}
}
