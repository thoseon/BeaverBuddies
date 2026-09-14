using System;
using Timberborn.Common;
using Timberborn.GameSceneLoading;
using Timberborn.MapStateSystem;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.Persistence;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.HazardousWeatherSystem;

public class DroughtWeather : ISaveableSingleton, ILoadableSingleton, IHazardousWeather
{
	private static readonly SingletonKey DroughtWeatherKey = new SingletonKey("DroughtWeather");

	private static readonly PropertyKey<int> MinDroughtDurationKey = new PropertyKey<int>("MinDroughtDuration");

	private static readonly PropertyKey<int> MaxDroughtDurationKey = new PropertyKey<int>("MaxDroughtDuration");

	private static readonly PropertyKey<float> HandicapMultiplierKey = new PropertyKey<float>("HandicapMultiplier");

	private static readonly PropertyKey<int> HandicapCyclesKey = new PropertyKey<int>("HandicapCycles");

	private readonly ISingletonLoader _singletonLoader;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly MapEditorMode _mapEditorMode;

	private readonly ISceneLoader _sceneLoader;

	private int _minDroughtDuration;

	private int _maxDroughtDuration;

	private float _handicapMultiplier;

	private int _handicapCycles;

	public string Id => DroughtWeatherKey.Name;

	public DroughtWeather(ISingletonLoader singletonLoader, IRandomNumberGenerator randomNumberGenerator, MapEditorMode mapEditorMode, ISceneLoader sceneLoader)
	{
		_singletonLoader = singletonLoader;
		_randomNumberGenerator = randomNumberGenerator;
		_mapEditorMode = mapEditorMode;
		_sceneLoader = sceneLoader;
	}

	public void Initialize(int minDroughtDuration, int maxDroughtDuration, float handicapMultiplier, int handicapCycles)
	{
		_minDroughtDuration = minDroughtDuration;
		_maxDroughtDuration = maxDroughtDuration;
		_handicapMultiplier = handicapMultiplier;
		_handicapCycles = handicapCycles;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			IObjectSaver singleton = singletonSaver.GetSingleton(DroughtWeatherKey);
			singleton.Set(MinDroughtDurationKey, _minDroughtDuration);
			singleton.Set(MaxDroughtDurationKey, _maxDroughtDuration);
			singleton.Set(HandicapMultiplierKey, _handicapMultiplier);
			singleton.Set(HandicapCyclesKey, _handicapCycles);
		}
	}

	public void Load()
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			if (_singletonLoader.TryGetSingleton(DroughtWeatherKey, out var objectLoader))
			{
				Initialize(objectLoader.Get(MinDroughtDurationKey), objectLoader.Get(MaxDroughtDurationKey), objectLoader.Get(HandicapMultiplierKey), objectLoader.Get(HandicapCyclesKey));
				return;
			}
			GameModeSpec gameMode = _sceneLoader.GetSceneParameters<GameSceneParameters>().NewGameConfiguration.GameMode;
			Initialize(gameMode.DroughtDuration.Min, gameMode.DroughtDuration.Max, gameMode.DroughtDurationHandicapMultiplier, gameMode.DroughtDurationHandicapCycles);
		}
	}

	public int GetDurationAtCycle(int cycle)
	{
		float handicapMultiplier = HazardousWeatherHelper.GetHandicapMultiplier(cycle, _handicapMultiplier, _handicapCycles);
		float inclusiveMin = handicapMultiplier * (float)_minDroughtDuration;
		float inclusiveMax = handicapMultiplier * (float)_maxDroughtDuration;
		int num = (int)Math.Round(_randomNumberGenerator.Range(inclusiveMin, inclusiveMax), MidpointRounding.AwayFromZero);
		if (_minDroughtDuration > 0)
		{
			num = Math.Max(num, 1);
		}
		return num;
	}
}
