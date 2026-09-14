using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.HousingBatchControl;

[Context("Game")]
internal class HousingBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly HousingBatchControlTab _housingBatchControlTab;

		public BatchControlModuleProvider(HousingBatchControlTab housingBatchControlTab)
		{
			_housingBatchControlTab = housingBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_housingBatchControlTab, 2);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<HousingBatchControlTab>().AsSingleton();
		Bind<HousingBatchControlRowFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
