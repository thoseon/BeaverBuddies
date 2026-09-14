using Timberborn.GameCycleSystem;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherService : ISaveableSingleton, ILoadableSingleton, ICycleDuration
{
	private static readonly SingletonKey HazardousWeatherServiceKey = new SingletonKey("HazardousWeatherService");

	private static readonly PropertyKey<int> HazardousWeatherDurationKey = new PropertyKey<int>("HazardousWeatherDuration");

	private static readonly PropertyKey<bool> IsDroughtKey = new PropertyKey<bool>("IsDrought");

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapEditorMode _mapEditorMode;

	private readonly DroughtWeather _droughtWeather;

	private readonly BadtideWeather _badtideWeather;

	private readonly HazardousWeatherRandomizer _hazardousWeatherRandomizer;

	private readonly HazardousWeatherHistory _hazardousWeatherHistory;

	public IHazardousWeather CurrentCycleHazardousWeather { get; private set; }

	public int HazardousWeatherDuration { get; private set; }

	public int DurationInDays => HazardousWeatherDuration;

	public HazardousWeatherService(EventBus eventBus, ISingletonLoader singletonLoader, MapEditorMode mapEditorMode, DroughtWeather droughtWeather, BadtideWeather badtideWeather, HazardousWeatherRandomizer hazardousWeatherRandomizer, HazardousWeatherHistory hazardousWeatherHistory)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
		_mapEditorMode = mapEditorMode;
		_droughtWeather = droughtWeather;
		_badtideWeather = badtideWeather;
		_hazardousWeatherRandomizer = hazardousWeatherRandomizer;
		_hazardousWeatherHistory = hazardousWeatherHistory;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			IObjectSaver singleton = singletonSaver.GetSingleton(HazardousWeatherServiceKey);
			singleton.Set(HazardousWeatherDurationKey, HazardousWeatherDuration);
			singleton.Set(IsDroughtKey, CurrentCycleHazardousWeather == _droughtWeather);
		}
	}

	public void Load()
	{
		if (!_mapEditorMode.IsMapEditor && _singletonLoader.TryGetSingleton(HazardousWeatherServiceKey, out var objectLoader))
		{
			HazardousWeatherDuration = objectLoader.Get(HazardousWeatherDurationKey);
			if (objectLoader.Get(IsDroughtKey))
			{
				CurrentCycleHazardousWeather = _droughtWeather;
			}
			else
			{
				CurrentCycleHazardousWeather = _badtideWeather;
			}
		}
	}

	public void SetForCycle(int cycle)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			CurrentCycleHazardousWeather = _hazardousWeatherRandomizer.GetRandomWeatherForCycle(cycle);
			int cyclesCount = _hazardousWeatherHistory.GetCyclesCount(CurrentCycleHazardousWeather.Id);
			HazardousWeatherDuration = CurrentCycleHazardousWeather.GetDurationAtCycle(cyclesCount + 1);
			_eventBus.Post(new HazardousWeatherSelectedEvent(CurrentCycleHazardousWeather, HazardousWeatherDuration));
		}
	}

	public void StartHazardousWeather()
	{
		_eventBus.Post(new HazardousWeatherStartedEvent(CurrentCycleHazardousWeather));
	}

	public void EndHazardousWeather()
	{
		_eventBus.Post(new HazardousWeatherEndedEvent(CurrentCycleHazardousWeather));
	}
}
