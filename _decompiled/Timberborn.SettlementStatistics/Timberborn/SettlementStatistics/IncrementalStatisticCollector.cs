using System.Collections.Generic;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.SettlementStatistics;

public class IncrementalStatisticCollector : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey IncrementalStatisticCollectorKey = new SingletonKey("IncrementalStatisticCollector");

	private static readonly ListKey<IncrementalStatistic> SettlementStatisticsKey = new ListKey<IncrementalStatistic>("SettlementStatistics");

	private readonly IncrementalStatisticSerializer _incrementalStatisticSerializer;

	private readonly ISingletonLoader _singletonLoader;

	private readonly Dictionary<string, IncrementalStatistic> _settlementStatistics = new Dictionary<string, IncrementalStatistic>();

	public IncrementalStatisticCollector(IncrementalStatisticSerializer incrementalStatisticSerializer, ISingletonLoader singletonLoader)
	{
		_incrementalStatisticSerializer = incrementalStatisticSerializer;
		_singletonLoader = singletonLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(IncrementalStatisticCollectorKey).Set(SettlementStatisticsKey, _settlementStatistics.Values, _incrementalStatisticSerializer);
	}

	public void Load()
	{
		if (!_singletonLoader.TryGetSingleton(IncrementalStatisticCollectorKey, out var objectLoader))
		{
			return;
		}
		foreach (IncrementalStatistic item in objectLoader.Get(SettlementStatisticsKey, _incrementalStatisticSerializer))
		{
			_settlementStatistics.Add(item.Id, item);
		}
	}

	public int GetOrDefault(string id)
	{
		if (!_settlementStatistics.TryGetValue(id, out var value))
		{
			return 0;
		}
		return value.Value;
	}

	public void Increment(string id)
	{
		if (!_settlementStatistics.TryGetValue(id, out var value))
		{
			value = new IncrementalStatistic(id, 0);
			_settlementStatistics.Add(id, value);
		}
		value.Increment();
	}
}
