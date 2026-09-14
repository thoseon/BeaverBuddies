using Bindito.Core;
using Timberborn.BatchControl;

namespace Timberborn.CharactersBatchControl;

[Context("Game")]
internal class CharactersBatchControlConfigurator : Configurator
{
	private class BatchControlModuleProvider : IProvider<BatchControlModule>
	{
		private readonly CharacterBatchControlTab _characterBatchControlTab;

		public BatchControlModuleProvider(CharacterBatchControlTab characterBatchControlTab)
		{
			_characterBatchControlTab = characterBatchControlTab;
		}

		public BatchControlModule Get()
		{
			BatchControlModule.Builder builder = new BatchControlModule.Builder();
			builder.AddTab(_characterBatchControlTab, 1);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<CharacterBatchControlTab>().AsSingleton();
		Bind<CharacterBatchControlRowFactory>().AsSingleton();
		MultiBind<BatchControlModule>().ToProvider<BatchControlModuleProvider>().AsSingleton();
	}
}
