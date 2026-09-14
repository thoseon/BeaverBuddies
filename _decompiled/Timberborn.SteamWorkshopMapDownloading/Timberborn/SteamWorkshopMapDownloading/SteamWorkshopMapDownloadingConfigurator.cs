using Bindito.Core;
using Timberborn.MapItemsUI;

namespace Timberborn.SteamWorkshopMapDownloading;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class SteamWorkshopMapDownloadingConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<ICustomMapItemFactory>().To<SteamWorkshopMapItemFactory>().AsSingleton();
		Bind<SteamMapRepositoryChangeNotifier>().AsSingleton();
	}
}
