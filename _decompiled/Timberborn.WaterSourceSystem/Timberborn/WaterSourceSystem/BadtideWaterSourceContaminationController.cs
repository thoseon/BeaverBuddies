using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GameCycleSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.WaterSourceSystem;

internal class BadtideWaterSourceContaminationController : TickableComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly float HalfDay = 0.5f;

	private readonly EventBus _eventBus;

	private readonly WeatherService _weatherService;

	private readonly GameCycleService _gameCycleService;

	private WaterSourceContamination _waterSourceContamination;

	private HazardousWeatherObserver _hazardousWeatherObserver;

	public BadtideWaterSourceContaminationController(EventBus eventBus, WeatherService weatherService, GameCycleService gameCycleService)
	{
		_eventBus = eventBus;
		_weatherService = weatherService;
		_gameCycleService = gameCycleService;
	}

	public void Awake()
	{
		_waterSourceContamination = GetComponent<WaterSourceContamination>();
		_hazardousWeatherObserver = GetComponent<HazardousWeatherObserver>();
		_eventBus.Register(this);
	}

	public void InitializeEntity()
	{
		if (_hazardousWeatherObserver.IsBadtideWeather)
		{
			EnableComponent();
			UpdateBadtideContamination();
		}
		else
		{
			DisableComponent();
		}
	}

	public override void Tick()
	{
		UpdateBadtideContamination();
	}

	[OnEvent]
	public void OnHazardousWeatherStarted(HazardousWeatherStartedEvent hazardousWeatherStartedEvent)
	{
		if (hazardousWeatherStartedEvent.HazardousWeather is BadtideWeather)
		{
			EnableComponent();
		}
	}

	[OnEvent]
	public void OnHazardousWeatherEnded(HazardousWeatherEndedEvent hazardousWeatherEndedEvent)
	{
		if (hazardousWeatherEndedEvent.HazardousWeather is BadtideWeather)
		{
			_waterSourceContamination.ResetContamination();
			DisableComponent();
		}
	}

	private void UpdateBadtideContamination()
	{
		_waterSourceContamination.SetContamination(GetCurrentContamination());
	}

	private float GetCurrentContamination()
	{
		float num = _gameCycleService.PartialCycleDay - (float)_weatherService.HazardousWeatherStartCycleDay;
		if (num < HalfDay)
		{
			return EvaluateContamination(num);
		}
		float num2 = (float)(_weatherService.CycleLengthInDays + 1) - _gameCycleService.PartialCycleDay;
		if (num2 < HalfDay)
		{
			return EvaluateContamination(num2);
		}
		return 1f;
	}

	private static float EvaluateContamination(float time)
	{
		return HyperbolicSecant(17f * (time - HalfDay)) * 0.5f + 0.5f;
	}

	private static float HyperbolicSecant(float x)
	{
		return 2f / (MathF.Exp(x) + MathF.Exp(0f - x));
	}
}
