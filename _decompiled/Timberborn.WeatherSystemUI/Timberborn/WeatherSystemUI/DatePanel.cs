using Timberborn.CoreUI;
using Timberborn.GameCycleSystem;
using Timberborn.HazardousWeatherSystemUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.UILayoutSystem;
using Timberborn.WeatherSystem;
using UnityEngine.UIElements;

namespace Timberborn.WeatherSystemUI;

internal class DatePanel : ILoadableSingleton
{
	private static readonly string WeatherTemperateLocKey = "Weather.Temperate";

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WeatherService _weatherService;

	private readonly TimestampFormatter _timestampFormatter;

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly EventBus _eventBus;

	private readonly HazardousWeatherUIHelper _hazardousWeatherUIHelper;

	private readonly GameCycleService _gameCycleService;

	private VisualElement _root;

	private Label _text;

	private string _tooltipText;

	private string _currentIconClass;

	public DatePanel(UILayout uiLayout, VisualElementLoader visualElementLoader, WeatherService weatherService, TimestampFormatter timestampFormatter, ILoc loc, ITooltipRegistrar tooltipRegistrar, EventBus eventBus, HazardousWeatherUIHelper hazardousWeatherUIHelper, GameCycleService gameCycleService)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_weatherService = weatherService;
		_timestampFormatter = timestampFormatter;
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
		_eventBus = eventBus;
		_hazardousWeatherUIHelper = hazardousWeatherUIHelper;
		_gameCycleService = gameCycleService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_root = _visualElementLoader.LoadVisualElement("Game/DatePanel");
		_tooltipRegistrar.Register(_root, () => _tooltipText);
		_text = _root.Q<Label>("Text");
		UpdatePanel();
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopRight(_root, 5);
	}

	[OnEvent]
	public void OnDaytimeStart(DaytimeStartEvent daytimeStartEvent)
	{
		UpdatePanel();
	}

	private void UpdatePanel()
	{
		UpdateIcon();
		UpdateText();
	}

	private void UpdateIcon()
	{
		if (!string.IsNullOrEmpty(_currentIconClass))
		{
			_root.RemoveFromClassList(_currentIconClass);
			_currentIconClass = null;
		}
		if (_weatherService.IsHazardousWeather)
		{
			_currentIconClass = _hazardousWeatherUIHelper.IconClass;
			_root.AddToClassList(_currentIconClass);
		}
	}

	private void UpdateText()
	{
		_text.text = _timestampFormatter.FormatLongLocalized(_gameCycleService.Cycle, _gameCycleService.CycleDay);
		_tooltipText = _loc.T(_weatherService.IsHazardousWeather ? _hazardousWeatherUIHelper.NameLocKey : WeatherTemperateLocKey);
	}
}
