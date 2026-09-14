using Bindito.Core;

namespace Timberborn.SteamWorkshopMapDownloadingUI;

[Context("MainMenu")]
[Context("MapEditor")]
internal class SteamWorkshopMapDownloadingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamWorkshopMapDownloader>().AsSingleton();
	}
}
