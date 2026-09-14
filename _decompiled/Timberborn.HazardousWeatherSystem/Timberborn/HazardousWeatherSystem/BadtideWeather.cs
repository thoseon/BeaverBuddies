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

public class BadtideWeather : ISaveableSingleton, ILoadableSingleton, IHazardousWeather
{
	private static readonly SingletonKey BadtideWeatherKey = new SingletonKey("BadtideWeather");

	private static readonly PropertyKey<int> MinDurationKey = new PropertyKey<int>("MinBadtideWeatherDuration");

	private static readonly PropertyKey<int> MaxDurationKey = new PropertyKey<int>("MaxBadtideWeatherDuration");

	private static readonly PropertyKey<float> HandicapMultiplierKey = new PropertyKey<float>("HandicapMultiplier");

	private static readonly PropertyKey<int> HandicapCyclesKey = new PropertyKey<int>("HandicapCycles");

	private static readonly PropertyKey<int> CyclesBeforeRandomizingKey = new PropertyKey<int>("CyclesBeforeRandomizing");

	private static readonly PropertyKey<float> ChanceForBadtideWeatherKey = new PropertyKey<float>("ChanceBadtideWeather");

	private readonly ISingletonLoader _singletonLoader;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly MapEditorMode _mapEditorMode;

	private readonly ISceneLoader _sceneLoader;

	private readonly GameModeSpecService _gameModeSpecService;

	private int _minDuration;

	private int _maxDuration;

	private float _handicapMultiplier;

	private int _handicapCycles;

	private int _cyclesBeforeRandomizingBadtideWeather;

	public float ChanceForBadtide { get; private set; }

	public string Id => BadtideWeatherKey.Name;

	public BadtideWeather(ISingletonLoader singletonLoader, IRandomNumberGenerator randomNumberGenerator, MapEditorMode mapEditorMode, ISceneLoader sceneLoader, GameModeSpecService gameModeSpecService)
	{
		_singletonLoader = singletonLoader;
		_randomNumberGenerator = randomNumberGenerator;
		_mapEditorMode = mapEditorMode;
		_sceneLoader = sceneLoader;
		_gameModeSpecService = gameModeSpecService;
	}

	public void Initialize(GameModeSpec gameMode)
	{
		Initialize(gameMode.BadtideDuration.Min, gameMode.BadtideDuration.Max, gameMode.BadtideDurationHandicapMultiplier, gameMode.BadtideDurationHandicapCycles, gameMode.CyclesBeforeRandomizingBadtide, gameMode.ChanceForBadtide);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			IObjectSaver singleton = singletonSaver.GetSingleton(BadtideWeatherKey);
			singleton.Set(MinDurationKey, _minDuration);
			singleton.Set(MaxDurationKey, _maxDuration);
			singleton.Set(HandicapMultiplierKey, _handicapMultiplier);
			singleton.Set(HandicapCyclesKey, _handicapCycles);
			singleton.Set(CyclesBeforeRandomizingKey, _cyclesBeforeRandomizingBadtideWeather);
			singleton.Set(ChanceForBadtideWeatherKey, ChanceForBadtide);
		}
	}

	public void Load()
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			GameSceneParameters sceneParameters;
			if (_singletonLoader.TryGetSingleton(BadtideWeatherKey, out var objectLoader))
			{
				Initialize(objectLoader.Get(MinDurationKey), objectLoader.Get(MaxDurationKey), objectLoader.Get(HandicapMultiplierKey), objectLoader.Get(HandicapCyclesKey), objectLoader.Get(CyclesBeforeRandomizingKey), objectLoader.Get(ChanceForBadtideWeatherKey));
			}
			else if (_sceneLoader.TryGetSceneParameters<GameSceneParameters>(out sceneParameters) && sceneParameters.NewGame)
			{
				Initialize(sceneParameters.NewGameConfiguration.GameMode);
			}
			else
			{
				Initialize(_gameModeSpecService.GetDefaultSpec());
			}
		}
	}

	public bool CanOccurAtCycle(int cycle)
	{
		return cycle > _cyclesBeforeRandomizingBadtideWeather;
	}

	public int GetDurationAtCycle(int cycle)
	{
		float handicapMultiplier = HazardousWeatherHelper.GetHandicapMultiplier(cycle, _handicapMultiplier, _handicapCycles);
		float inclusiveMin = handicapMultiplier * (float)_minDuration;
		float inclusiveMax = handicapMultiplier * (float)_maxDuration;
		int num = (int)Math.Round(_randomNumberGenerator.Range(inclusiveMin, inclusiveMax), MidpointRounding.AwayFromZero);
		if (_minDuration > 0)
		{
			num = Math.Max(num, 1);
		}
		return num;
	}

	private void Initialize(int minDuration, int maxDuration, float handicapMultiplier, int handicapCycles, int cyclesBeforeRandomizingBadtide, float chanceForBadtide)
	{
		_minDuration = minDuration;
		_maxDuration = maxDuration;
		_handicapMultiplier = handicapMultiplier;
		_handicapCycles = handicapCycles;
		_cyclesBeforeRandomizingBadtideWeather = cyclesBeforeRandomizingBadtide;
		ChanceForBadtide = chanceForBadtide;
	}
}
