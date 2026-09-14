using Bindito.Core;

namespace Timberborn.SteamOverlaySystem;

[Context("Game")]
[Context("MainMenu")]
[Context("MapEditor")]
internal class SteamOverlaySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamOverlayInputBlocker>().AsSingleton();
		Bind<SteamOverlayOpener>().AsSingleton();
	}
}
