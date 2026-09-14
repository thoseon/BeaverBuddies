using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.RuinsModelShuffling;

[Context("MapEditor")]
internal class RuinsModelShufflingConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly RuinModelShufflingFragment _ruinModelShufflingFragment;

		public EntityPanelModuleProvider(RuinModelShufflingFragment ruinModelShufflingFragment)
		{
			_ruinModelShufflingFragment = ruinModelShufflingFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_ruinModelShufflingFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<RuinModelShufflingFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
