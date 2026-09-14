using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.SettlementStatistics;

internal class DaysPassedStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public DaysPassedStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnDaytimeStartEvent(DaytimeStartEvent daytimeStartEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.DaysPassed);
	}
}
