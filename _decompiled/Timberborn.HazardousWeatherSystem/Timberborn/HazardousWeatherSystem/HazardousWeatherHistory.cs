using System.Collections.Generic;
using System.Linq;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherHistory : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey HazardousWeatherHistoryKey = new SingletonKey("HazardousWeatherHistory");

	private static readonly ListKey<HazardousWeatherHistoryData> HistoryDataKey = new ListKey<HazardousWeatherHistoryData>("HistoryData");

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private readonly HazardousWeatherHistoryDataSerializer _hazardousWeatherHistoryDataSerializer;

	private readonly List<HazardousWeatherHistoryData> _history = new List<HazardousWeatherHistoryData>();

	private readonly Dictionary<string, int> _cyclesCount = new Dictionary<string, int>();

	public int CurrentStreak { get; private set; }

	public string CurrentStreakId => _history.Last().HazardousWeatherId;

	public HazardousWeatherHistory(EventBus eventBus, ISingletonLoader singletonLoader, HazardousWeatherHistoryDataSerializer hazardousWeatherHistoryDataSerializer)
	{
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
		_hazardousWeatherHistoryDataSerializer = hazardousWeatherHistoryDataSerializer;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(HazardousWeatherHistoryKey).Set(HistoryDataKey, _history, _hazardousWeatherHistoryDataSerializer);
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(HazardousWeatherHistoryKey, out var objectLoader))
		{
			if (objectLoader.Has(HistoryDataKey))
			{
				foreach (HazardousWeatherHistoryData item in objectLoader.Get(HistoryDataKey, _hazardousWeatherHistoryDataSerializer))
				{
					AddHazardousWeatherData(item);
				}
			}
			if (_history.Any())
			{
				CalculateStreakFromHistory();
			}
		}
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnHazardousWeatherSelected(HazardousWeatherSelectedEvent hazardousWeatherSelectedEvent)
	{
		AddHazardousWeatherData(new HazardousWeatherHistoryData(hazardousWeatherSelectedEvent.SelectedWeather.Id, hazardousWeatherSelectedEvent.Duration));
		CalculateStreakFromHistory();
	}

	public bool TryGetPreviousHazardousWeatherData(out HazardousWeatherHistoryData hazardousWeatherHistoryData)
	{
		if (_history.Count > 1)
		{
			List<HazardousWeatherHistoryData> history = _history;
			hazardousWeatherHistoryData = history[history.Count - 2];
			return true;
		}
		hazardousWeatherHistoryData = null;
		return false;
	}

	public int GetCyclesCount(string hazardousWeatherId)
	{
		return _cyclesCount.GetValueOrDefault(hazardousWeatherId, 0);
	}

	private void AddHazardousWeatherData(HazardousWeatherHistoryData hazardousWeatherHistoryData)
	{
		_history.Add(hazardousWeatherHistoryData);
		string hazardousWeatherId = hazardousWeatherHistoryData.HazardousWeatherId;
		if (!_cyclesCount.TryAdd(hazardousWeatherId, 1))
		{
			_cyclesCount[hazardousWeatherId]++;
		}
	}

	private void CalculateStreakFromHistory()
	{
		CurrentStreak = 0;
		string currentStreakId = CurrentStreakId;
		int num = _history.Count - 1;
		while (num >= 0 && currentStreakId == _history[num].HazardousWeatherId)
		{
			CurrentStreak++;
			num--;
		}
	}
}
