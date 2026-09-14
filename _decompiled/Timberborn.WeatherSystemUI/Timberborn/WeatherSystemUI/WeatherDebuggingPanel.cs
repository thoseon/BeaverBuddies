using Timberborn.DebuggingUI;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.WeatherSystemUI;

internal class WeatherDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly TemperateWeatherDurationService _temperateWeatherDurationService;

	private readonly HazardousWeatherService _hazardousWeatherService;

	public WeatherDebuggingPanel(DebuggingPanel debuggingPanel, TemperateWeatherDurationService temperateWeatherDurationService, HazardousWeatherService hazardousWeatherService)
	{
		_debuggingPanel = debuggingPanel;
		_temperateWeatherDurationService = temperateWeatherDurationService;
		_hazardousWeatherService = hazardousWeatherService;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Weather");
	}

	public string GetText()
	{
		int temperateWeatherDuration = _temperateWeatherDurationService.TemperateWeatherDuration;
		return $"Temperate weather duration: {temperateWeatherDuration}" + "\nHazardous weather: " + _hazardousWeatherService.CurrentCycleHazardousWeather.GetType().Name + $"\nHazardous weather duration: {_hazardousWeatherService.HazardousWeatherDuration}";
	}
}
