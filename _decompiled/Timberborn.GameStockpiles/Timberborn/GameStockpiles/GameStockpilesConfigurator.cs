using Bindito.Core;
using Timberborn.DuplicationSystem;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.InventoryNeedSystem;
using Timberborn.Stockpiles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GameStockpiles;

[Context("Game")]
internal class GameStockpilesConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDecorator<Stockpile, Emptiable>();
			builder.AddDecorator<Stockpile, HaulCandidate>();
			builder.AddDecorator<Stockpile, InventoryNeedBehavior>();
			builder.AddDecorator<Stockpile, EmptyInventoriesWorkplaceBehavior>();
			builder.AddDecorator<Stockpile, RemoveUnwantedStockWorkplaceBehavior>();
			builder.AddDecorator<FixedStockpileSpec, FixedStockpileRemover>();
			builder.AddDecorator<FixedStockpileSpec, UnreachableFixedStockpileStatus>();
			builder.AddDecorator<FixedStockpileSpec, DuplicationBlocker>();
			builder.AddDecorator<FixedStockpileSpec, FixedStockpileDeletionBlocker>();
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<FixedStockpileRemover>().AsTransient();
		Bind<UnreachableFixedStockpileStatus>().AsTransient();
		Bind<FixedStockpileDeletionBlocker>().AsTransient();
		Bind<StockpileInventoryBehaviorInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
