using Timberborn.InventoryNeedSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.SettlementStatistics;

internal class WaterConsumedStatisticCollector : ILoadableSingleton
{
	private static readonly string WaterGoodId = "Water";

	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public WaterConsumedStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnGoodConsumed(GoodConsumedEvent goodConsumedEvent)
	{
		if (goodConsumedEvent.GoodId == WaterGoodId)
		{
			_incrementalStatisticCollector.Increment(StatisticIds.WaterConsumed);
		}
	}
}
