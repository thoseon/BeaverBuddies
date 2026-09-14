using Bindito.Core;

namespace Timberborn.SteamWorkshopMapUploadingUI;

[Context("MapEditor")]
internal class SteamWorkshopMapUploadingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamWorkshopUploadableMapFactory>().AsSingleton();
		Bind<SteamWorkshopUploadMapPanelOpener>().AsSingleton();
		Bind<SteamWorkshopMapDataService>().AsSingleton();
	}
}
