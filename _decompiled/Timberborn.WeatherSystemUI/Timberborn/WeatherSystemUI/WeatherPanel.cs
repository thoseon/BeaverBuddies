using System;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.GameCycleSystem;
using Timberborn.GameSound;
using Timberborn.HazardousWeatherSystemUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.UILayoutSystem;
using Timberborn.WeatherSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WeatherSystemUI;

internal class WeatherPanel : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly string TemperateWeatherLocKey = "Weather.Temperate";

	private static readonly string BlinkingClass = "weather-panel--blink";

	private static readonly string ApproachingClass = "weather-approaching";

	private static readonly string InProgressClass = "weather-in-progress";

	private readonly UILayout _uiLayout;

	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WeatherService _weatherService;

	private readonly ILoc _loc;

	private readonly GameUISoundController _gameUISoundController;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly HazardousWeatherUIHelper _hazardousWeatherUIHelper;

	private readonly HazardousWeatherApproachingTimer _hazardousWeatherApproachingTimer;

	private readonly GameCycleService _gameCycleService;

	private readonly ISpecService _specService;

	private WeatherPanelSpec _weatherPanelSpec;

	private VisualElement _root;

	private Label _forecastCounter;

	private SimpleProgressBar _simpleProgressBar;

	private float _secondsToNextBlink;

	private int _remainingBlinks;

	private bool _midBlink;

	private bool _pausedUntilTimeUnpaused;

	private bool _startBlinkingIfUnpaused;

	private string _tooltipText;

	private string _hazardousWeatherClass;

	private readonly Phrase _forecastCounterPhrase = Phrase.New().FormatDays<float>("F1");

	public WeatherPanel(UILayout uiLayout, EventBus eventBus, VisualElementLoader visualElementLoader, WeatherService weatherService, ILoc loc, GameUISoundController gameUISoundController, ITooltipRegistrar tooltipRegistrar, HazardousWeatherUIHelper hazardousWeatherUIHelper, HazardousWeatherApproachingTimer hazardousWeatherApproachingTimer, GameCycleService gameCycleService, ISpecService specService)
	{
		_uiLayout = uiLayout;
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_weatherService = weatherService;
		_loc = loc;
		_gameUISoundController = gameUISoundController;
		_tooltipRegistrar = tooltipRegistrar;
		_hazardousWeatherUIHelper = hazardousWeatherUIHelper;
		_hazardousWeatherApproachingTimer = hazardousWeatherApproachingTimer;
		_gameCycleService = gameCycleService;
		_specService = specService;
	}

	public void Load()
	{
		_weatherPanelSpec = _specService.GetSingleSpec<WeatherPanelSpec>();
		_root = _visualElementLoader.LoadVisualElement("Game/WeatherPanel");
		_tooltipRegistrar.Register(_root, () => _tooltipText);
		_simpleProgressBar = _root.Q<SimpleProgressBar>("Progress");
		_forecastCounter = _root.Q<Label>("ForecastCounter");
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_pausedUntilTimeUnpaused && Time.deltaTime > 0f)
		{
			_pausedUntilTimeUnpaused = false;
		}
		if (!_pausedUntilTimeUnpaused)
		{
			UpdatePanel();
			if (_startBlinkingIfUnpaused)
			{
				_startBlinkingIfUnpaused = false;
				StartBlinking();
			}
		}
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopRight(_root, 6);
		UpdatePanel();
		_pausedUntilTimeUnpaused = true;
	}

	[OnEvent]
	public void OnHazardousWeatherApproaching(HazardousWeatherApproachingEvent hazardousWeatherApproachingEvent)
	{
		_startBlinkingIfUnpaused = true;
	}

	private void UpdatePanel()
	{
		int hazardousWeatherStartCycleDay = _weatherService.HazardousWeatherStartCycleDay;
		float partialCycleDay = _gameCycleService.PartialCycleDay;
		UpdateHazardousWeatherClasses();
		if (_weatherService.IsHazardousWeather)
		{
			SetHazardousWeatherUI(partialCycleDay, hazardousWeatherStartCycleDay);
		}
		else if (_hazardousWeatherApproachingTimer.GetProgress() > 0f)
		{
			float approachingHazardUI = (float)hazardousWeatherStartCycleDay - partialCycleDay;
			SetApproachingHazardUI(approachingHazardUI);
		}
		else
		{
			SetPanelContent(_loc.T(TemperateWeatherLocKey), 0f, 0f);
		}
	}

	private void UpdateHazardousWeatherClasses()
	{
		_root.RemoveFromClassList(ApproachingClass);
		_root.RemoveFromClassList(InProgressClass);
		if (!string.IsNullOrEmpty(_hazardousWeatherClass))
		{
			_root.RemoveFromClassList(_hazardousWeatherClass);
		}
		_hazardousWeatherClass = _hazardousWeatherUIHelper.InProgressClass;
		_root.AddToClassList(_hazardousWeatherClass);
	}

	private void SetHazardousWeatherUI(float partialCycleDay, int hazardousWeatherStartCycleDay)
	{
		float num = partialCycleDay - (float)hazardousWeatherStartCycleDay;
		float forecastCount = (float)_weatherService.HazardousWeatherDuration - num;
		float progressBarValue = num / (float)_weatherService.HazardousWeatherDuration;
		string inProgressLocKey = _hazardousWeatherUIHelper.InProgressLocKey;
		SetPanelContent(_loc.T(inProgressLocKey), progressBarValue, forecastCount);
		_root.AddToClassList(InProgressClass);
	}

	private void SetApproachingHazardUI(float daysToHazardousWeather)
	{
		_root.AddToClassList(ApproachingClass);
		bool blink = _remainingBlinks > 0 && NextBlinkingBarState();
		float progress = _hazardousWeatherApproachingTimer.GetProgress();
		SetPanelContent(_loc.T(_hazardousWeatherUIHelper.ApproachingLocKey), progress, daysToHazardousWeather, blink);
	}

	private void SetPanelContent(string forecast, float progressBarValue, float forecastCount, bool blink = false)
	{
		_simpleProgressBar.SetProgress(Math.Max(progressBarValue, 0f));
		_root.EnableInClassList(BlinkingClass, blink);
		_forecastCounter.ToggleDisplayStyle(forecastCount > 0f);
		_forecastCounter.text = _loc.T(_forecastCounterPhrase, forecastCount);
		_tooltipText = forecast;
	}

	private void StartBlinking()
	{
		_remainingBlinks = _weatherPanelSpec.NumberOfBlinks * 2 - 1;
		_midBlink = true;
		_secondsToNextBlink = _weatherPanelSpec.SecondsBetweenBlinks + Time.unscaledDeltaTime;
		_gameUISoundController.PlayBlinkingSound();
	}

	private bool NextBlinkingBarState()
	{
		_secondsToNextBlink -= Time.unscaledDeltaTime;
		if (_secondsToNextBlink <= 0f)
		{
			_secondsToNextBlink = _weatherPanelSpec.SecondsBetweenBlinks;
			_remainingBlinks--;
			_midBlink = !_midBlink;
		}
		return _midBlink;
	}
}
