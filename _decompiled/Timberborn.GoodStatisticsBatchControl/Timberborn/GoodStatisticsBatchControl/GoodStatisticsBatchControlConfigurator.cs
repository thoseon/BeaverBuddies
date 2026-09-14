using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.GoodStatisticsBatchControl;

[Context("Game")]
public class GoodStatisticsBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly GoodStatisticsBatchControlTab _goodStatisticsBatchControlTab;

		public BatchControlModuleProvider(GoodStatisticsBatchControlTab goodStatisticsBatchControlTab)
		{
			_goodStatisticsBatchControlTab = goodStatisticsBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_goodStatisticsBatchControlTab, 90);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GoodStatisticsBatchControlRowGroupFactory>().AsSingleton();
		Bind<GoodStatisticsBatchControlTab>().AsSingleton();
		Bind<GoodStatisticsGroupFactory>().AsSingleton();
		Bind<GoodStatisticsRowItemFactory>().AsSingleton();
		Bind<GoodStatisticsBatchControlItemFactory>().AsSingleton();
		Bind<GoodStatisticsTypeSelector>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
