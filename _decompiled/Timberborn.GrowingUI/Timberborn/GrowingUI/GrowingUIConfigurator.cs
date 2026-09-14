using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.GrowingUI;

[Context("Game")]
internal class GrowingUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly GrowableFragment _growableFragment;

		public EntityPanelModuleProvider(GrowableFragment growableFragment)
		{
			_growableFragment = growableFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_growableFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GrowableFragment>().AsSingleton();
		Bind<GrowableToolPanelItemFactory>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
