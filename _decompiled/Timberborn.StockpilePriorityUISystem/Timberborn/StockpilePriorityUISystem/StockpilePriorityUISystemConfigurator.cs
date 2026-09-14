using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.StockpilePriorityUISystem;

[Context("Game")]
internal class StockpilePriorityUISystemConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly StockpilePriorityFragment _stockpilePriorityFragment;

		public EntityPanelModuleProvider(StockpilePriorityFragment stockpilePriorityFragment)
		{
			_stockpilePriorityFragment = stockpilePriorityFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_stockpilePriorityFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<StockpilePriorityFragment>().AsSingleton();
		Bind<StockpilePriorityToggleFactory>().AsSingleton();
		Bind<StockpilePriorityBatchControlRowItemFactory>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
