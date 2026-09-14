using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.SimpleOutputBuildingsUI;

[Context("Game")]
internal class SimpleOutputBuildingsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly SimpleOutputInventoryFragment _simpleOutputInventoryFragment;

		public EntityPanelModuleProvider(SimpleOutputInventoryFragment simpleOutputInventoryFragment)
		{
			_simpleOutputInventoryFragment = simpleOutputInventoryFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_simpleOutputInventoryFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<SimpleOutputInventoryFragmentEnabler>().AsTransient();
		Bind<SimpleOutputInventoryFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
