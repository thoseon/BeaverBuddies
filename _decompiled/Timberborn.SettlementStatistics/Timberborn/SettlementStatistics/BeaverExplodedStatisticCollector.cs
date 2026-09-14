using Timberborn.Explosions;
using Timberborn.SingletonSystem;

namespace Timberborn.SettlementStatistics;

internal class BeaverExplodedStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public BeaverExplodedStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnMortalDiedFromExplosion(MortalDiedFromExplosionEvent mortalDiedFromExplosionEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.BeaversExploded);
	}
}
