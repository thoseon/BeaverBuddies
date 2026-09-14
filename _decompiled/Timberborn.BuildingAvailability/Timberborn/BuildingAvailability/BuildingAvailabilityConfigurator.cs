using Bindito.Core;
using Timberborn.ToolSystem;

namespace Timberborn.BuildingAvailability;

[Context("Game")]
internal class BuildingAvailabilityConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingAvailabilityValidator>().AsSingleton();
		MultiBind<IToolDisabler>().To<BuildingAvailabilityToolDisabler>().AsSingleton();
	}
}
