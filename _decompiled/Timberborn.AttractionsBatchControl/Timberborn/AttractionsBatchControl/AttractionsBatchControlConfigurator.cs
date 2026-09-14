using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.AttractionsBatchControl;

[Context("Game")]
internal class AttractionsBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly AttractionsBatchControlTab _attractionsBatchControlTab;

		public BatchControlModuleProvider(AttractionsBatchControlTab attractionsBatchControlTab)
		{
			_attractionsBatchControlTab = attractionsBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_attractionsBatchControlTab, 6);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<AttractionsBatchControlTab>().AsSingleton();
		Bind<AttractionsBatchControlRowFactory>().AsSingleton();
		Bind<GoodConsumingAttractionBatchControlRowItemFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
