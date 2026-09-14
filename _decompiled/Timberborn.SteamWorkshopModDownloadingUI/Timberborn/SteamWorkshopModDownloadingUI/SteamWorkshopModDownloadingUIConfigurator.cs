using Bindito.Core;

namespace Timberborn.SteamWorkshopModDownloadingUI;

[Context("MainMenu")]
internal class SteamWorkshopModDownloadingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamWorkshopModDownloader>().AsSingleton();
	}
}
