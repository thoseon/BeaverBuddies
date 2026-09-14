using Timberborn.SingletonSystem;
using Timberborn.TailDecalSystem;

namespace Timberborn.SettlementStatistics;

internal class TailsPaintedStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public TailsPaintedStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnTailDecalApplied(TailDecalAppliedEvent tailDecalAppliedEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.TailsPainted);
	}
}
