using System;
using Timberborn.GameCycleSystem;
using Timberborn.GameSound;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.HazardousWeatherSystemUI;

internal class HazardousWeatherSoundPlayer : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly GameUISoundController _gameUISoundController;

	public HazardousWeatherSoundPlayer(EventBus eventBus, GameUISoundController gameUISoundController)
	{
		_eventBus = eventBus;
		_gameUISoundController = gameUISoundController;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnHazardousWeatherStarted(HazardousWeatherStartedEvent hazardousWeatherStartedEvent)
	{
		if (hazardousWeatherStartedEvent.HazardousWeather is BadtideWeather)
		{
			_gameUISoundController.PlayBadtideStartedSound();
			return;
		}
		if (hazardousWeatherStartedEvent.HazardousWeather is DroughtWeather)
		{
			_gameUISoundController.PlayDroughtStartedSound();
			return;
		}
		throw new ArgumentException("No start sound for weather type: " + hazardousWeatherStartedEvent.HazardousWeather.Id);
	}

	[OnEvent]
	public void OnCycleStarted(CycleStartedEvent cycleStartedEvent)
	{
		if (cycleStartedEvent.Cycle > 1)
		{
			_gameUISoundController.PlayTemperateWeatherStartedSound();
		}
	}
}
