using Bindito.Core;

namespace Timberborn.Population;

[Context("Game")]
internal class PopulationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PopulationDataCollector>().AsSingleton();
		Bind<PopulationService>().AsSingleton();
	}
}
