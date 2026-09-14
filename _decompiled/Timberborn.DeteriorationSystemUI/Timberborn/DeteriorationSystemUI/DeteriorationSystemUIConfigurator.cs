using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.DeteriorationSystemUI;

[Context("Game")]
internal class DeteriorationSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DeteriorableFragment _deteriorableFragment;

		private readonly DeteriorableDebugFragment _deteriorableDebugFragment;

		public EntityPanelModuleProvider(DeteriorableFragment deteriorableFragment, DeteriorableDebugFragment deteriorableDebugFragment)
		{
			_deteriorableFragment = deteriorableFragment;
			_deteriorableDebugFragment = deteriorableDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_deteriorableFragment);
			builder.AddDiagnosticFragment(_deteriorableDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DeteriorableFragment>().AsSingleton();
		Bind<DeteriorableBatchControlRowItemFactory>().AsSingleton();
		Bind<DeteriorableDebugFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
