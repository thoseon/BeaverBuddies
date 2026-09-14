using Timberborn.Beavers;
using Timberborn.SingletonSystem;

namespace Timberborn.SettlementStatistics;

internal class BeaverBornStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public BeaverBornStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBeaverBorn(BeaverBornEvent beaverBornEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.BeaversBorn);
	}
}
