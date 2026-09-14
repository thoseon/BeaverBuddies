using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.WaterSourceSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.GameWaterSourceSystem;

public class HazardousWeatherWaterSource : BaseComponent, IAwakableComponent, IInitializableEntity, IWaterStrengthModifier
{
	private readonly WeatherService _weatherService;

	private readonly HazardousWeatherService _hazardousWeatherService;

	private HazardousWeatherWaterSourceSpec _hazardousWeatherWaterSourceSpec;

	private WaterSource _waterSource;

	private bool _activeInEditor;

	public HazardousWeatherWaterSource(WeatherService weatherService, HazardousWeatherService hazardousWeatherService)
	{
		_weatherService = weatherService;
		_hazardousWeatherService = hazardousWeatherService;
	}

	public void Awake()
	{
		_hazardousWeatherWaterSourceSpec = GetComponent<HazardousWeatherWaterSourceSpec>();
		_waterSource = GetComponent<WaterSource>();
	}

	public void InitializeEntity()
	{
		_waterSource.AddWaterStrengthModifier(this);
	}

	public float GetStrengthModifier()
	{
		return ShouldBeActive() ? 1 : 0;
	}

	private bool ShouldBeActive()
	{
		if (_weatherService.IsHazardousWeather)
		{
			return _hazardousWeatherWaterSourceSpec.ActiveInHazardousWeather.Contains(_hazardousWeatherService.CurrentCycleHazardousWeather.Id);
		}
		return false;
	}
}
