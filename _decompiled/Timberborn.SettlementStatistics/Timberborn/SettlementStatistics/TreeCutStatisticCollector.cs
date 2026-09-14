using Timberborn.Forestry;
using Timberborn.SingletonSystem;

namespace Timberborn.SettlementStatistics;

internal class TreeCutStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public TreeCutStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnTreeCut(TreeCutEvent treeCutEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.TreesCut);
	}
}
