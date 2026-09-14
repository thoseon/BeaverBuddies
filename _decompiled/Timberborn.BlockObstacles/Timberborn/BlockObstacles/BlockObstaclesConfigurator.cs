using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BlockObstacles;

[Context("Game")]
internal class BlockObstaclesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LayeredBlockObstacleVisualizer>().AsTransient();
		Bind<BlockOccupier>().AsTransient();
		Bind<LayeredBlockObstacle>().AsTransient();
		Bind<BlockOccupationLayerFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<LayeredBlockObstacleSpec, LayeredBlockObstacle>();
		builder.AddDecorator<LayeredBlockObstacleVisualizerSpec, LayeredBlockObstacleVisualizer>();
		builder.AddDecorator<BlockOccupierSpec, BlockOccupier>();
		return builder.Build();
	}
}
