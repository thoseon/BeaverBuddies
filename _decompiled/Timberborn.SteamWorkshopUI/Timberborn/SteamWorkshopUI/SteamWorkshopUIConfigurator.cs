using Bindito.Core;

namespace Timberborn.SteamWorkshopUI;

[Context("MainMenu")]
[Context("MapEditor")]
internal class SteamWorkshopUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamWorkshopUploadPanel>().AsSingleton();
		Bind<VisibilityDropdownProvider>().AsSingleton();
		Bind<UploadPanelElements>().AsSingleton();
		Bind<SteamWorkshopUploadProgress>().AsSingleton();
		Bind<UploadPanelTags>().AsSingleton();
	}
}
