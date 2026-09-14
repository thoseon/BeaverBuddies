using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.StorageBatchControl;

[Context("Game")]
internal class StorageBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly StorageBatchControlTab _storageBatchControlTab;

		public BatchControlModuleProvider(StorageBatchControlTab storageBatchControlTab)
		{
			_storageBatchControlTab = storageBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_storageBatchControlTab, 4);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<StorageBatchControlTab>().AsSingleton();
		Bind<StorageBatchControlRowFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
