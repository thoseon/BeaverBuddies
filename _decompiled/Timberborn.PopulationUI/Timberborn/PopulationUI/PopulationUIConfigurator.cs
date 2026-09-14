using Bindito.Core;

namespace Timberborn.PopulationUI;

[Context("Game")]
internal class PopulationUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<HousingDataRowFactory>().AsSingleton();
		Bind<PopulationDataRowFactory>().AsSingleton();
		Bind<PopulationPanel>().AsSingleton();
		Bind<PopulationServiceDistrictSwitcher>().AsSingleton();
		Bind<WorkplaceDataRowFactory>().AsSingleton();
	}
}
