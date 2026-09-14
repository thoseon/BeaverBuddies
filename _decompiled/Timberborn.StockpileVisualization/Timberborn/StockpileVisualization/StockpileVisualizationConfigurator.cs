using Bindito.Core;
using Timberborn.Rendering;
using Timberborn.Stockpiles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.StockpileVisualization;

[Context("Game")]
[Context("MapEditor")]
internal class StockpileVisualizationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GoodVisualization>().AsTransient();
		Bind<StockpileBannerSetter>().AsTransient();
		Bind<StockpileGoodColumnVisualizer>().AsTransient();
		Bind<StockpileGoodPileVisualizer>().AsTransient();
		Bind<StockpilePlaneVisualizer>().AsTransient();
		Bind<StockpileVisualizers>().AsTransient();
		Bind<StockpileVisualizationUpdater>().AsTransient();
		Bind<GoodVisualizationSpecService>().AsSingleton();
		Bind<GoodColumnVariantsService>().AsSingleton();
		Bind<GoodPileVariantsService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Stockpile, StockpileBannerSetter>();
		builder.AddDecorator<Stockpile, StockpileVisualizationUpdater>();
		builder.AddDecorator<IStockpileVisualizer, StockpileVisualizers>();
		builder.AddDecorator<IStockpileVisualizer, EntityMaterials>();
		builder.AddDecorator<StockpileVisualizers, GoodVisualization>();
		builder.AddDecorator<StockpileGoodColumnVisualizerSpec, StockpileGoodColumnVisualizer>();
		builder.AddDecorator<StockpileGoodPileVisualizerSpec, StockpileGoodPileVisualizer>();
		builder.AddDecorator<StockpilePlaneVisualizerSpec, StockpilePlaneVisualizer>();
		return builder.Build();
	}
}
