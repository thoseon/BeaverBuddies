using Bindito.Core;

namespace Timberborn.ConstructionGuidelinesUI;

[Context("Game")]
[Context("MapEditor")]
internal class ConstructionGuidelinesUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ConstructionGuidelinesTogglePanel>().AsSingleton();
		Bind<ConstructionModeGuidelinesShower>().AsSingleton();
	}
}
