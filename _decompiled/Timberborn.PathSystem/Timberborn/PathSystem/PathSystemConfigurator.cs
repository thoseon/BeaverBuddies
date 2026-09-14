using Bindito.Core;
using Timberborn.Navigation;
using Timberborn.TemplateInstantiation;

namespace Timberborn.PathSystem;

[Context("Game")]
internal class PathSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DrivewayModels>().AsTransient();
		Bind<DynamicPathModel>().AsTransient();
		Bind<DynamicPathModelUpdater>().AsTransient();
		Bind<PathModelTypeEnforcer>().AsTransient();
		Bind<SpiralStairsHeightProvider>().AsTransient();
		Bind<StraightStairsHeightProvider>().AsTransient();
		Bind<DrivewayModelInstantiator>().AsSingleton();
		Bind<IPathService>().To<PathService>().AsSingleton();
		Bind<IConnectionService>().To<ConnectionService>().AsSingleton();
		MultiBind<IPathTransformer>().To<StairsPathTransformer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DrivewayModelsSpec, DrivewayModels>();
		builder.AddDecorator<DynamicPathModelSpec, DynamicPathModel>();
		builder.AddDecorator<StraightStairsSpec, StraightStairsHeightProvider>();
		builder.AddDecorator<SpiralStairsSpec, SpiralStairsHeightProvider>();
		builder.AddDecorator<PathModelTypeEnforcerSpec, PathModelTypeEnforcer>();
		return builder.Build();
	}
}
