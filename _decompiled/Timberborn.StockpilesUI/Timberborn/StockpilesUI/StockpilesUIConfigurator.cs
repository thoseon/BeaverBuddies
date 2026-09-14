using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Illumination;
using Timberborn.Stockpiles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.StockpilesUI;

[Context("Game")]
internal class StockpilesUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly StockpileInventoryFragment _stockpileInventoryFragment;

		private readonly StockpileInventoryDebugFragment _stockpileInventoryDebugFragment;

		public EntityPanelModuleProvider(StockpileInventoryFragment stockpileInventoryFragment, StockpileInventoryDebugFragment stockpileInventoryDebugFragment)
		{
			_stockpileInventoryFragment = stockpileInventoryFragment;
			_stockpileInventoryDebugFragment = stockpileInventoryDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_stockpileInventoryFragment);
			builder.AddDiagnosticFragment(_stockpileInventoryDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<StockpileDropdownProvider>().AsTransient();
		Bind<StockpileOverlayItemAdder>().AsTransient();
		Bind<NoGoodAllowedStatus>().AsTransient();
		Bind<UnwantedStockStatus>().AsTransient();
		Bind<StockpileDescriber>().AsTransient();
		Bind<StockpileInventoryFragment>().AsSingleton();
		Bind<StockpileBatchControlRowItemFactory>().AsSingleton();
		Bind<IGoodSelectionController>().To<GoodSelectionController>().AsSingleton();
		Bind<StockpileOverlay>().AsSingleton();
		Bind<StockpileOverlayShower>().AsSingleton();
		Bind<StockpileGoodSelectionBoxFactory>().AsSingleton();
		Bind<StockpileOverlayTogglePanel>().AsSingleton();
		Bind<StockpileOverlayHider>().AsSingleton();
		Bind<StockpileInventoryDebugFragment>().AsSingleton();
		Bind<GoodSelectionBoxRowFactory>().AsSingleton();
		Bind<GoodSelectionBoxItemFactory>().AsSingleton();
		Bind<StockpileOptionsService>().AsSingleton();
		Bind<StockpileGoodSelectionBoxItemsFactory>().AsSingleton();
		Bind<GoodStockpilesTooltipFactory>().AsSingleton();
		Bind<OverlayGoodSelectionController>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Stockpile, StockpileDropdownProvider>();
		builder.AddDecorator<Stockpile, StockpileOverlayItemAdder>();
		builder.AddDecorator<Stockpile, StockpileDescriber>();
		builder.AddDecorator<Stockpile, NoGoodAllowedStatus>();
		builder.AddDecorator<Stockpile, UnwantedStockStatus>();
		builder.AddDecorator<StockpileIlluminatorSpec, Illuminator>();
		return builder.Build();
	}
}
