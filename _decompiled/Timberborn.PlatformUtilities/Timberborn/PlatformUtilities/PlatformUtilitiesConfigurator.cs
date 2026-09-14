using Bindito.Core;

namespace Timberborn.PlatformUtilities;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class PlatformUtilitiesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IExplorerOpener>().To<ExplorerOpener>().AsSingleton();
	}
}
