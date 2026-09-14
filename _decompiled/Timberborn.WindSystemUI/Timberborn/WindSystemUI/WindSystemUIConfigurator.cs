using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;

namespace Timberborn.WindSystemUI;

[Context("Game")]
internal class WindSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly WeathervaneFragment _weathervaneFragment;

		public EntityPanelModuleProvider(WeathervaneFragment weathervaneFragment)
		{
			_weathervaneFragment = weathervaneFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_weathervaneFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<WeathervaneFragment>().AsSingleton();
		MultiBind<IDevModule>().To<DebugWindDevModule>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
