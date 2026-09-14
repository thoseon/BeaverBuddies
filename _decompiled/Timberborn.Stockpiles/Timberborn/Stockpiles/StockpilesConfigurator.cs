using Bindito.Core;
using Timberborn.InventorySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Stockpiles;

[Context("Game")]
[Context("MapEditor")]
internal class StockpilesConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly StockpileInventoryInitializer _stockpileInventoryInitializer;

		public TemplateModuleProvider(StockpileInventoryInitializer stockpileInventoryInitializer)
		{
			_stockpileInventoryInitializer = stockpileInventoryInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDecorator<StockpileSpec, Stockpile>();
			builder.AddDecorator<Stockpile, SingleGoodAllower>();
			builder.AddDecorator<FixedStockpileSpec, FixedStockpile>();
			builder.AddDedicatedDecorator(_stockpileInventoryInitializer);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<Stockpile>().AsTransient();
		Bind<FixedStockpile>().AsTransient();
		Bind<StockpileInventoryInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
