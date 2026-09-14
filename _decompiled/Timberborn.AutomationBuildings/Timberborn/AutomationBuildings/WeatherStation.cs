using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.GameCycleSystem;
using Timberborn.HazardousWeatherSystem;
using Timberborn.Persistence;
using Timberborn.WeatherSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class WeatherStation : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<WeatherStation>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	private static readonly ComponentKey WeatherStationKey = new ComponentKey("WeatherStation");

	private static readonly PropertyKey<WeatherStationMode> ModeKey = new PropertyKey<WeatherStationMode>("Mode");

	private static readonly PropertyKey<bool> EarlyActivationEnabledKey = new PropertyKey<bool>("EarlyActivationEnabled");

	private static readonly PropertyKey<int> EarlyActivationHoursKey = new PropertyKey<int>("EarlyActivationHours");

	private readonly WeatherService _weatherService;

	private readonly HazardousWeatherService _hazardousWeatherService;

	private readonly GameCycleService _gameCycleService;

	private WeatherStationSpec _weatherStationSpec;

	private Automator _automator;

	private float _sampledPartialCycleDay;

	private int _sampledCycleLengthInDays;

	private int _sampledHazardousWeatherStartCycleDay;

	private bool _sampledIsDrought;

	private bool _sampledIsBadtide;

	public WeatherStationMode Mode { get; private set; }

	public bool EarlyActivationEnabled { get; private set; }

	public int EarlyActivationHours { get; private set; }

	public float MaxEarlyActivationHours => _weatherStationSpec.MaxEarlyActivationHours;

	private float EffectiveEarlyActivationDays
	{
		get
		{
			if (!EarlyActivationEnabled)
			{
				return 0f;
			}
			return (float)EarlyActivationHours / 24f;
		}
	}

	private bool WithinActivationTimeTemperate
	{
		get
		{
			if (!(_sampledPartialCycleDay < (float)_sampledHazardousWeatherStartCycleDay))
			{
				return _sampledPartialCycleDay >= (float)(_sampledCycleLengthInDays + 1) - EffectiveEarlyActivationDays;
			}
			return true;
		}
	}

	private bool WithinActivationTimeHazardous => _sampledPartialCycleDay >= (float)_sampledHazardousWeatherStartCycleDay - EffectiveEarlyActivationDays;

	public WeatherStation(WeatherService weatherService, HazardousWeatherService hazardousWeatherService, GameCycleService gameCycleService)
	{
		_weatherService = weatherService;
		_hazardousWeatherService = hazardousWeatherService;
		_gameCycleService = gameCycleService;
	}

	public void Awake()
	{
		_weatherStationSpec = GetComponent<WeatherStationSpec>();
		_automator = GetComponent<Automator>();
	}

	public void SetMode(WeatherStationMode value)
	{
		if (Mode != value)
		{
			Mode = value;
			UpdateOutputState();
		}
	}

	public void SetEarlyActivationEnabled(bool value)
	{
		if (EarlyActivationEnabled != value)
		{
			EarlyActivationEnabled = value;
			UpdateOutputState();
		}
	}

	public void SetEarlyActivationHours(int value)
	{
		if (EarlyActivationHours != value)
		{
			EarlyActivationHours = value;
			UpdateOutputState();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(WeatherStationKey);
		component.Set(ModeKey, Mode);
		component.Set(EarlyActivationEnabledKey, EarlyActivationEnabled);
		component.Set(EarlyActivationHoursKey, EarlyActivationHours);
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WeatherStationKey, out var objectLoader))
		{
			Mode = (objectLoader.Has(ModeKey) ? objectLoader.Get(ModeKey) : WeatherStationMode.Temperate);
			EarlyActivationEnabled = objectLoader.Has(EarlyActivationEnabledKey) && objectLoader.Get(EarlyActivationEnabledKey);
			EarlyActivationHours = (objectLoader.Has(EarlyActivationHoursKey) ? objectLoader.Get(EarlyActivationHoursKey) : 0);
		}
	}

	public void DuplicateFrom(WeatherStation source)
	{
		Mode = source.Mode;
		EarlyActivationEnabled = source.EarlyActivationEnabled;
		EarlyActivationHours = source.EarlyActivationHours;
		UpdateOutputState();
	}

	public void Sample()
	{
		_sampledPartialCycleDay = _gameCycleService.PartialCycleDay;
		_sampledCycleLengthInDays = _weatherService.CycleLengthInDays;
		_sampledHazardousWeatherStartCycleDay = _weatherService.HazardousWeatherStartCycleDay;
		_sampledIsDrought = _hazardousWeatherService.CurrentCycleHazardousWeather is DroughtWeather;
		_sampledIsBadtide = _hazardousWeatherService.CurrentCycleHazardousWeather is BadtideWeather;
		UpdateOutputState();
	}

	private void UpdateOutputState()
	{
		Automator automator = _automator;
		automator.SetState(Mode switch
		{
			WeatherStationMode.Temperate => WithinActivationTimeTemperate, 
			WeatherStationMode.Drought => _sampledIsDrought && WithinActivationTimeHazardous, 
			WeatherStationMode.Badtide => _sampledIsBadtide && WithinActivationTimeHazardous, 
			_ => throw new ArgumentOutOfRangeException(), 
		});
	}
}
