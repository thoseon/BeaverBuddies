using Timberborn.BotUpkeep;
using Timberborn.SingletonSystem;

namespace Timberborn.SettlementStatistics;

internal class BotsManufacturedStatisticCollector : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public BotsManufacturedStatisticCollector(EventBus eventBus, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_eventBus = eventBus;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBotManufactured(BotManufacturedEvent botManufacturedEvent)
	{
		_incrementalStatisticCollector.Increment(StatisticIds.BotsManufactured);
	}
}
