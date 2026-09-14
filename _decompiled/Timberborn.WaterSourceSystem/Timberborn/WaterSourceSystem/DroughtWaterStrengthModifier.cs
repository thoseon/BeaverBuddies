using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GameCycleSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.WaterSourceSystem;

internal class DroughtWaterStrengthModifier : BaseComponent, IAwakableComponent, IInitializableEntity, IWaterStrengthModifier
{
	private readonly WaterStrengthService _waterStrengthService;

	private readonly HazardousWeatherService _hazardousWeatherService;

	private readonly WeatherService _weatherService;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly HazardousWeatherHistory _hazardousWeatherHistory;

	private readonly DroughtWeather _droughtWeather;

	private readonly GameCycleService _gameCycleService;

	private WaterSource _waterSource;

	private float _oldSpecifiedStrength;

	private bool IsCycleWithDrought => _hazardousWeatherService.CurrentCycleHazardousWeather is DroughtWeather;

	private bool PreviousCycleHadDrought
	{
		get
		{
			if (_hazardousWeatherHistory.TryGetPreviousHazardousWeatherData(out var hazardousWeatherHistoryData) && hazardousWeatherHistoryData.Duration > 0)
			{
				return hazardousWeatherHistoryData.HazardousWeatherId == _droughtWeather.Id;
			}
			return false;
		}
	}

	public DroughtWaterStrengthModifier(WaterStrengthService waterStrengthService, HazardousWeatherService hazardousWeatherService, WeatherService weatherService, IDayNightCycle dayNightCycle, HazardousWeatherHistory hazardousWeatherHistory, DroughtWeather droughtWeather, GameCycleService gameCycleService)
	{
		_waterStrengthService = waterStrengthService;
		_hazardousWeatherService = hazardousWeatherService;
		_weatherService = weatherService;
		_dayNightCycle = dayNightCycle;
		_hazardousWeatherHistory = hazardousWeatherHistory;
		_droughtWeather = droughtWeather;
		_gameCycleService = gameCycleService;
	}

	public void Awake()
	{
		_waterSource = GetComponent<WaterSource>();
	}

	public void InitializeEntity()
	{
		Enable();
	}

	public void Enable()
	{
		_waterSource.AddWaterStrengthModifier(this);
	}

	public void Disable()
	{
		_waterSource.RemoveWaterStrengthModifier(this);
	}

	public float GetStrengthModifier()
	{
		if (_weatherService.IsHazardousWeather)
		{
			return (!IsCycleWithDrought) ? 1 : 0;
		}
		return GetTemperateWeatherModifier();
	}

	private float GetTemperateWeatherModifier()
	{
		float transitionTime = GetTransitionTime();
		float num = (float)_weatherService.HazardousWeatherStartCycleDay - transitionTime;
		if (ShouldStopWaterFlow(num))
		{
			float transitionProgress = _gameCycleService.PartialCycleDay - num;
			return 1f - GetModifier(transitionProgress, transitionTime);
		}
		float num2 = _gameCycleService.PartialCycleDay - 1f;
		if (PreviousCycleHadDrought && num2 < transitionTime)
		{
			return GetModifier(num2, transitionTime);
		}
		return 1f;
	}

	private float GetTransitionTime()
	{
		return _waterSource.SpecifiedStrength / (_dayNightCycle.DayLengthInSeconds * _waterStrengthService.MaxWaterSourceChangePerSecond);
	}

	private bool ShouldStopWaterFlow(float transitionStartCycleDay)
	{
		if (IsCycleWithDrought && _weatherService.HazardousWeatherDuration > 0)
		{
			return _gameCycleService.PartialCycleDay >= transitionStartCycleDay;
		}
		return false;
	}

	private float GetModifier(float transitionProgress, float transitionTime)
	{
		float num = (1f - _waterStrengthService.MinWaterSourceChangeScaler) * (transitionProgress / transitionTime) + _waterStrengthService.MinWaterSourceChangeScaler;
		return transitionProgress * _dayNightCycle.DayLengthInSeconds * _waterStrengthService.MaxWaterSourceChangePerSecond * num / _waterSource.SpecifiedStrength;
	}
}
