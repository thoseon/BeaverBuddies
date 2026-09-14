using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.CarryingUI;

[Context("Game")]
internal class CarryingUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly GoodCarrierFragment _goodCarrierFragment;

		public EntityPanelModuleProvider(GoodCarrierFragment goodCarrierFragment)
		{
			_goodCarrierFragment = goodCarrierFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_goodCarrierFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GoodCarrierFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
