using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WaterSystem;

[Context("Game")]
[Context("MapEditor")]
internal class WaterSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WaterMapBoundary>().AsTransient();
		Bind<WaterSimulator>().AsSingleton();
		Bind<ThreadSafeWaterMap>().AsSingleton();
		Bind<IThreadSafeWaterMap>().ToExisting<ThreadSafeWaterMap>();
		Bind<WaterSourceRegistry>().AsSingleton();
		Bind<IWaterService>().To<WaterService>().AsSingleton();
		Bind<INonThreadSafeWaterService>().To<NonThreadSafeWaterService>().AsSingleton();
		Bind<WaterChangeService>().AsSingleton();
		Bind<IWaterRemovalService>().ToExisting<WaterChangeService>();
		Bind<WaterMapLoader>().AsSingleton();
		Bind<ColumnOutflowsPackedListSerializer>().AsSingleton();
		Bind<WaterColumnPackedListSerializer>().AsSingleton();
		Bind<FlowVectorCalculator>().AsSingleton();
		Bind<FlowLimiterService>().AsSingleton();
		Bind<IFlowLimiterService>().ToExisting<FlowLimiterService>();
		Bind<WaterMapBoundaryService>().AsSingleton();
		Bind<WaterColumnRetriever>().AsSingleton();
		Bind<WaterFlowRetriever>().AsSingleton();
		Bind<FlowLimitCalculator>().AsSingleton();
		Bind<WaterSimulationTaskStarter>().AsSingleton();
		Bind<WaterDepthSetter>().AsSingleton();
		Bind<MutableWaterColumnRetriever>().AsSingleton();
		Bind<WaterSimulationMigrator>().AsSingleton();
		Bind<WaterOverflowCalculator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<IWaterSource, WaterMapBoundary>();
		return builder.Build();
	}
}
