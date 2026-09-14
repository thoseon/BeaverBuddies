using Bindito.Core;

namespace Timberborn.BuildingDoorsteps;

[Context("Game")]
internal class BuildingDoorstepsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingDoorstepSpawner>().AsSingleton();
	}
}
