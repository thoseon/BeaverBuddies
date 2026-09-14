using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.PowerBatchControl;

[Context("Game")]
internal class PowerBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly MechanicalBatchControlTab _mechanicalBatchControlTab;

		public BatchControlModuleProvider(MechanicalBatchControlTab mechanicalBatchControlTab)
		{
			_mechanicalBatchControlTab = mechanicalBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_mechanicalBatchControlTab, 5);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<MechanicalBatchControlTab>().AsSingleton();
		Bind<MechanicalBatchControlRowFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
