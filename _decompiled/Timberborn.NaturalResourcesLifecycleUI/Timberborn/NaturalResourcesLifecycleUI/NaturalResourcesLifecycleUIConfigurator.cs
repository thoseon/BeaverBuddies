using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.NaturalResourcesLifecycleUI;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesLifecycleUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DyingNaturalResourceFragment _dyingNaturalResourceFragment;

		public EntityPanelModuleProvider(DyingNaturalResourceFragment dyingNaturalResourceFragment)
		{
			_dyingNaturalResourceFragment = dyingNaturalResourceFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_dyingNaturalResourceFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DyingNaturalResourceFragment>().AsSingleton();
		Bind<DeadNaturalResourceDescriber>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
