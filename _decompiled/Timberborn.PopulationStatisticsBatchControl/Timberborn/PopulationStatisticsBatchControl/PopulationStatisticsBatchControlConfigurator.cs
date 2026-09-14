using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.PopulationStatisticsBatchControl;

[Context("Game")]
internal class PopulationStatisticsBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly PopulationStatisticsBatchControlTab _populationStatisticsBatchControlTab;

		public BatchControlModuleProvider(PopulationStatisticsBatchControlTab populationStatisticsBatchControlTab)
		{
			_populationStatisticsBatchControlTab = populationStatisticsBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_populationStatisticsBatchControlTab, 91);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<PopulationStatisticsBatchControlTab>().AsSingleton();
		Bind<PopulationStatisticsBatchControlRowGroupFactory>().AsSingleton();
		Bind<PopulationStatisticsRowItemFactory>().AsSingleton();
		Bind<PopulationStatisticsGraphFactory>().AsSingleton();
		Bind<PopulationGraphState>().AsSingleton();
		Bind<PopulationStatisticsTooltipRegistrar>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
