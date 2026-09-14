using Timberborn.Explosions;
using Timberborn.SingletonSystem;

namespace Timberborn.SettlementStatistics;

internal class DynamiteDetonatedStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public DynamiteDetonatedStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnDynamiteDetonated(DynamiteDetonatedEvent dynamiteDetonatedEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.DynamiteDetonated);
	}
}
