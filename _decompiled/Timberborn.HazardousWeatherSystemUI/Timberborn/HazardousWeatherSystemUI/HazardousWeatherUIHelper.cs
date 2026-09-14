using System;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.HazardousWeatherSystemUI;

public class HazardousWeatherUIHelper : ILoadableSingleton
{
	private readonly HazardousWeatherService _hazardousWeatherService;

	private readonly EventBus _eventBus;

	private readonly DroughtWeatherUISpecification _droughtWeatherUISpecification;

	private readonly BadtideWeatherUISpecification _badtideWeatherUISpecification;

	private IHazardousWeatherUISpecification _currentUISpecification;

	public string NameLocKey => _currentUISpecification.NameLocKey;

	public string ApproachingLocKey => _currentUISpecification.ApproachingLocKey;

	public string InProgressLocKey => _currentUISpecification.InProgressLocKey;

	public string StartedNotificationLocKey => _currentUISpecification.StartedNotificationLocKey;

	public string EndedNotificationLocKey => _currentUISpecification.EndedNotificationLocKey;

	public string InProgressClass => _currentUISpecification.InProgressClass;

	public string IconClass => _currentUISpecification.IconClass;

	public string NotificationBackgroundClass => _currentUISpecification.NotificationBackgroundClass;

	public HazardousWeatherUIHelper(HazardousWeatherService hazardousWeatherService, EventBus eventBus, DroughtWeatherUISpecification droughtWeatherUISpecification, BadtideWeatherUISpecification badtideWeatherUISpecification)
	{
		_hazardousWeatherService = hazardousWeatherService;
		_eventBus = eventBus;
		_droughtWeatherUISpecification = droughtWeatherUISpecification;
		_badtideWeatherUISpecification = badtideWeatherUISpecification;
	}

	public void Load()
	{
		_eventBus.Register(this);
		UpdateCurrentUISpecification();
	}

	[OnEvent]
	public void OnHazardousWeatherSelected(HazardousWeatherSelectedEvent hazardousWeatherSelectedEvent)
	{
		UpdateCurrentUISpecification();
	}

	private void UpdateCurrentUISpecification()
	{
		IHazardousWeather currentCycleHazardousWeather = _hazardousWeatherService.CurrentCycleHazardousWeather;
		IHazardousWeatherUISpecification currentUISpecification;
		if (!(currentCycleHazardousWeather is DroughtWeather))
		{
			if (!(currentCycleHazardousWeather is BadtideWeather))
			{
				throw new InvalidOperationException("No UI for weather: " + _hazardousWeatherService.CurrentCycleHazardousWeather);
			}
			currentUISpecification = _badtideWeatherUISpecification;
		}
		else
		{
			currentUISpecification = _droughtWeatherUISpecification;
		}
		_currentUISpecification = currentUISpecification;
	}
}
