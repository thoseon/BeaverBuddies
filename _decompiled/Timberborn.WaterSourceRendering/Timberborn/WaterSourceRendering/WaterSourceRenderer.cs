using Timberborn.ActivatorSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.WaterSourceSystem;
using Timberborn.WeatherSystem;

namespace Timberborn.WaterSourceRendering;

internal class WaterSourceRenderer : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly WaterSourceRenderingService _waterSourceRenderingService;

	private readonly WeatherService _weatherService;

	private readonly HazardousWeatherService _hazardousWeatherService;

	private WaterSource _waterSource;

	private TimedComponentActivator _timedComponentActivator;

	private bool _isDepthLimitedWaterSource;

	public bool CanBeRendered
	{
		get
		{
			if (!CanEmitWater())
			{
				return _waterSource.CurrentStrength > 0f;
			}
			return true;
		}
	}

	public WaterSourceRenderer(WaterSourceRenderingService waterSourceRenderingService, WeatherService weatherService, HazardousWeatherService hazardousWeatherService)
	{
		_waterSourceRenderingService = waterSourceRenderingService;
		_weatherService = weatherService;
		_hazardousWeatherService = hazardousWeatherService;
	}

	public void Awake()
	{
		_waterSource = GetComponent<WaterSource>();
		_timedComponentActivator = GetComponent<TimedComponentActivator>();
		_isDepthLimitedWaterSource = HasComponent<WaterDepthStrengthModifierSpec>();
	}

	public void InitializeEntity()
	{
		_waterSourceRenderingService.AddRenderer(this);
	}

	public void DeleteEntity()
	{
		_waterSourceRenderingService.RemoveRenderer(this);
	}

	private bool CanEmitWater()
	{
		if (_weatherService.IsHazardousWeather && _hazardousWeatherService.CurrentCycleHazardousWeather is DroughtWeather)
		{
			return false;
		}
		if (_timedComponentActivator.IsEnabled && !_timedComponentActivator.IsPastActivationTime)
		{
			return false;
		}
		return _isDepthLimitedWaterSource;
	}
}
