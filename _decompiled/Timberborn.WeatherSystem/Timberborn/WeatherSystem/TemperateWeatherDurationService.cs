using Timberborn.Common;
using Timberborn.GameCycleSystem;
using Timberborn.GameSceneLoading;
using Timberborn.MapStateSystem;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.Persistence;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.WeatherSystem;

public class TemperateWeatherDurationService : ISaveableSingleton, ILoadableSingleton, ICycleDuration
{
	private static readonly SingletonKey TemperateWeatherDurationServiceKey = new SingletonKey("TemperateWeatherDurationService");

	private static readonly PropertyKey<int> TemperateWeatherDurationKey = new PropertyKey<int>("TemperateWeatherDuration");

	private static readonly PropertyKey<int> MinTemperateWeatherDurationKey = new PropertyKey<int>("MinTemperateWeatherDuration");

	private static readonly PropertyKey<int> MaxTemperateWeatherDurationKey = new PropertyKey<int>("MaxTemperateWeatherDuration");

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly ISceneLoader _sceneLoader;

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapEditorMode _mapEditorMode;

	private int _minTemperateWeatherDuration;

	private int _maxTemperateWeatherDuration;

	public int TemperateWeatherDuration { get; private set; }

	public int DurationInDays => TemperateWeatherDuration;

	public TemperateWeatherDurationService(IRandomNumberGenerator randomNumberGenerator, ISceneLoader sceneLoader, ISingletonLoader singletonLoader, MapEditorMode mapEditorMode)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_sceneLoader = sceneLoader;
		_singletonLoader = singletonLoader;
		_mapEditorMode = mapEditorMode;
	}

	public void Initialize(int minTemperateWeatherDuration, int maxTemperateWeatherDuration)
	{
		_minTemperateWeatherDuration = minTemperateWeatherDuration;
		_maxTemperateWeatherDuration = maxTemperateWeatherDuration;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			IObjectSaver singleton = singletonSaver.GetSingleton(TemperateWeatherDurationServiceKey);
			singleton.Set(MinTemperateWeatherDurationKey, _minTemperateWeatherDuration);
			singleton.Set(MaxTemperateWeatherDurationKey, _maxTemperateWeatherDuration);
			singleton.Set(TemperateWeatherDurationKey, TemperateWeatherDuration);
		}
	}

	public void Load()
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			if (_singletonLoader.TryGetSingleton(TemperateWeatherDurationServiceKey, out var objectLoader))
			{
				Initialize(objectLoader.Get(MinTemperateWeatherDurationKey), objectLoader.Get(MaxTemperateWeatherDurationKey));
				TemperateWeatherDuration = objectLoader.Get(TemperateWeatherDurationKey);
			}
			else
			{
				MinMaxSpec<int> temperateWeatherDuration = _sceneLoader.GetSceneParameters<GameSceneParameters>().NewGameConfiguration.GameMode.TemperateWeatherDuration;
				Initialize(temperateWeatherDuration.Min, temperateWeatherDuration.Max);
			}
		}
	}

	public void SetForCycle(int cycle)
	{
		TemperateWeatherDuration = GenerateDuration();
	}

	public int GenerateDuration()
	{
		return _randomNumberGenerator.Range(_minTemperateWeatherDuration, _maxTemperateWeatherDuration + 1);
	}
}
