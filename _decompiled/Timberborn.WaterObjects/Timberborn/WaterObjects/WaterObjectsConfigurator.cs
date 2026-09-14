using Bindito.Core;
using Timberborn.BlockingSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WaterObjects;

[Context("Game")]
[Context("MapEditor")]
internal class WaterObjectsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FinishableHorizontalWaterObstacle>().AsTransient();
		Bind<FinishableWaterObstacle>().AsTransient();
		Bind<HorizontalWaterObstacle>().AsTransient();
		Bind<WaterObject>().AsTransient();
		Bind<WaterObstacle>().AsTransient();
		Bind<FloodableObject>().AsTransient();
		Bind<BlockableFloodableObject>().AsTransient();
		Bind<WaterObjectService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<FinishableHorizontalWaterObstacleSpec, FinishableHorizontalWaterObstacle>();
		builder.AddDecorator<FinishableHorizontalWaterObstacle, HorizontalWaterObstacle>();
		builder.AddDecorator<FinishableWaterObstacleSpec, FinishableWaterObstacle>();
		builder.AddDecorator<IWaterObjectSpecification, WaterObject>();
		builder.AddDecorator<WaterObstacleSpec, WaterObstacle>();
		builder.AddDecorator<FloodableObjectSpec, FloodableObject>();
		builder.AddDecorator<BlockableFloodableObjectSpec, BlockableFloodableObject>();
		builder.AddDecorator<BlockableFloodableObject, BlockableObject>();
		return builder.Build();
	}
}
