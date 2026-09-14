using Bindito.Core;

namespace Timberborn.SettlementStatistics;

[Context("Game")]
internal class SettlementStatisticsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BeaverBornStatisticCollector>().AsSingleton();
		Bind<BeaverExplodedStatisticCollector>().AsSingleton();
		Bind<BotsManufacturedStatisticCollector>().AsSingleton();
		Bind<ChippedTeethStatisticCollector>().AsSingleton();
		Bind<DynamiteDetonatedStatisticCollector>().AsSingleton();
		Bind<TreeCutStatisticCollector>().AsSingleton();
		Bind<WaterConsumedStatisticCollector>().AsSingleton();
		Bind<TailsPaintedStatisticCollector>().AsSingleton();
		Bind<IncrementalStatisticCollector>().AsSingleton();
		Bind<IncrementalStatisticSerializer>().AsSingleton();
		Bind<DaysPassedStatisticCollector>().AsSingleton();
	}
}
