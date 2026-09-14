using Bindito.Core;

namespace Timberborn.SimulationSystem;

[Context("Game")]
[Context("MapEditor")]
internal class SimulationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SimulationController>().AsSingleton();
	}
}
