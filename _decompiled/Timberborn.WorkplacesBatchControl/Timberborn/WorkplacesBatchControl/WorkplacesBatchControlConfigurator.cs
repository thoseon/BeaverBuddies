using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.WorkplacesBatchControl;

[Context("Game")]
internal class WorkplacesBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly WorkplacesBatchControlTab _workplacesBatchControlTab;

		public BatchControlModuleProvider(WorkplacesBatchControlTab workplacesBatchControlTab)
		{
			_workplacesBatchControlTab = workplacesBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_workplacesBatchControlTab, 3);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<WorkplacesBatchControlTab>().AsSingleton();
		Bind<WorkplacesBatchControlRowFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
