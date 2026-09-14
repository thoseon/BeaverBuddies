using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.GoodStackSystemUI;

[Context("Game")]
internal class GoodStackSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly GoodStackFragment _goodStackFragment;

		public EntityPanelModuleProvider(GoodStackFragment goodStackFragment)
		{
			_goodStackFragment = goodStackFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_goodStackFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GoodStackFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
