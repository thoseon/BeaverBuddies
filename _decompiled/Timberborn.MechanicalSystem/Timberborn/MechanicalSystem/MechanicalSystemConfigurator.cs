using Bindito.Core;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MechanicalSystem;

[Context("Game")]
[Context("MapEditor")]
internal class MechanicalSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MechanicalBuilding>().AsTransient();
		Bind<MechanicalNodeParticlesController>().AsTransient();
		Bind<MechanicalNode>().AsTransient();
		Bind<MechanicalNodeActuals>().AsTransient();
		Bind<TransputProvider>().AsTransient();
		Bind<MechanicalNodeTransformHeight>().AsTransient();
		Bind<MechanicalGraphManager>().AsSingleton();
		Bind<MechanicalGraphFactory>().AsSingleton();
		Bind<MechanicalGraphReorganizer>().AsSingleton();
		Bind<MechanicalGraphRegistry>().AsSingleton();
		Bind<BatteryCharger>().AsSingleton();
		Bind<BatteryDischarger>().AsSingleton();
		Bind<BatteryService>().AsSingleton();
		Bind<TransputMap>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<MechanicalNodeSpec, MechanicalNode>();
		builder.AddDecorator<MechanicalNode, MechanicalNodeActuals>();
		builder.AddDecorator<TransputProviderSpec, MechanicalNode>();
		builder.AddDecorator<TransputProviderSpec, TransputProvider>();
		builder.AddDecorator<MechanicalNodeTransformHeightSpec, MechanicalNodeTransformHeight>();
		builder.AddDecorator<MechanicalNodeParticlesControllerSpec, MechanicalNodeParticlesController>();
		builder.AddDecorator<MechanicalNodeParticlesController, ParticlesCache>();
		builder.AddDecorator<MechanicalBuildingSpec, MechanicalBuilding>();
		return builder.Build();
	}
}
