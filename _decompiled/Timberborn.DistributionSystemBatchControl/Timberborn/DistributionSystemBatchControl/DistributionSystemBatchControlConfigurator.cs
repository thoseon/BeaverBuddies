using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.DistributionSystemBatchControl;

[Context("Game")]
internal class DistributionSystemBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly DistributionBatchControlTab _distributionBatchControlTab;

		public BatchControlModuleProvider(DistributionBatchControlTab distributionBatchControlTab)
		{
			_distributionBatchControlTab = distributionBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_distributionBatchControlTab, 8);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DistributionBatchControlRowGroupFactory>().AsSingleton();
		Bind<DistributionBatchControlTab>().AsSingleton();
		Bind<DistributionSettingGroupFactory>().AsSingleton();
		Bind<DistributionSettingsRowItemFactory>().AsSingleton();
		Bind<DistrictDistributionControlRowItemFactory>().AsSingleton();
		Bind<GoodDistributionSettingItemFactory>().AsSingleton();
		Bind<ImportToggleFactory>().AsSingleton();
		Bind<ExportThresholdSliderFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
