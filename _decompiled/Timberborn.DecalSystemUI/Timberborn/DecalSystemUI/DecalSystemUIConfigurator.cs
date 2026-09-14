using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.DecalSystemUI;

[Context("Game")]
internal class DecalSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DecalSupplierFragment _decalSupplierFragment;

		private readonly FlippableDecalFragment _flippableDecalFragment;

		public EntityPanelModuleProvider(DecalSupplierFragment decalSupplierFragment, FlippableDecalFragment flippableDecalFragment)
		{
			_decalSupplierFragment = decalSupplierFragment;
			_flippableDecalFragment = flippableDecalFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_decalSupplierFragment);
			builder.AddMiddleFragment(_flippableDecalFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DecalSupplierFragment>().AsSingleton();
		Bind<DecalButtonFactory>().AsSingleton();
		Bind<DecalButtonContainer>().AsSingleton();
		Bind<FlippableDecalFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
