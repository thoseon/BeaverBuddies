using Bindito.Core;

namespace Timberborn.SteamWorkshop;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class SteamWorkshopConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamWorkshopItemCreator>().AsSingleton();
		Bind<SteamWorkshopItemUpdater>().AsSingleton();
		Bind<ItemInstalledNotifier>().AsSingleton();
		Bind<SteamWorkshopItemSerializer>().AsSingleton();
	}
}
