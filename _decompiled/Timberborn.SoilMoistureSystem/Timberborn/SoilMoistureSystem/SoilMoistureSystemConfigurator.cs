using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterSystem;

namespace Timberborn.SoilMoistureSystem;

[Context("Game")]
[Context("MapEditor")]
internal class SoilMoistureSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DryObject>().AsTransient();
		Bind<DryObjectModel>().AsTransient();
		Bind<SoilMoistureSimulator>().AsSingleton();
		Bind<ISoilMoistureService>().To<SoilMoistureService>().AsSingleton();
		Bind<SoilMoistureSimulationTaskStarter>().AsSingleton();
		Bind<WaterEvaporationMap>().AsSingleton();
		Bind<WaterEvaporationMapHelper>().AsSingleton();
		Bind<IThreadSafeWaterEvaporationMap>().ToExisting<WaterEvaporationMap>();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DryObjectModelSpec, DryObjectModel>();
		builder.AddDecorator<DryObjectSpec, DryObject>();
		return builder.Build();
	}
}
