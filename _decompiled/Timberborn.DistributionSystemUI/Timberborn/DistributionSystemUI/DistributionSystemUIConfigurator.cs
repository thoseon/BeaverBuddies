using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.DistributionSystemUI;

[Context("Game")]
internal class DistributionSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DistrictCrossingFragment _districtCrossingFragment;

		private readonly DistrictCrossingInventoryFragment _districtCrossingInventoryFragment;

		public EntityPanelModuleProvider(DistrictCrossingFragment districtCrossingFragment, DistrictCrossingInventoryFragment districtCrossingInventoryFragment)
		{
			_districtCrossingFragment = districtCrossingFragment;
			_districtCrossingInventoryFragment = districtCrossingInventoryFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddSideFragment(_districtCrossingFragment);
			builder.AddBottomFragment(_districtCrossingInventoryFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DistrictCrossingFragment>().AsSingleton();
		Bind<DistrictCrossingInventoryFragment>().AsSingleton();
		Bind<ImportGoodIconFactory>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
