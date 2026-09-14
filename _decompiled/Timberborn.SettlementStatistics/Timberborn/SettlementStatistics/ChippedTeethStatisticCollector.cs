using Timberborn.Healthcare;
using Timberborn.SingletonSystem;

namespace Timberborn.SettlementStatistics;

internal class ChippedTeethStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public ChippedTeethStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnTeethChipped(TeethChippedEvent teethChippedEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.ChippedTeeth);
	}
}
