using Bindito.Core;

namespace Timberborn.SoilContaminationSystem;

[Context("Game")]
[Context("MapEditor")]
internal class SoilContaminationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContaminatedObject>().AsTransient();
		Bind<SoilContaminationSimulator>().AsSingleton();
		Bind<ISoilContaminationService>().To<SoilContaminationService>().AsSingleton();
		Bind<SoilContaminationSimulationTaskStarter>().AsSingleton();
	}
}
