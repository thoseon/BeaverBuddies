using Bindito.Core;
using Timberborn.Modding;

namespace Timberborn.SteamWorkshopModDownloading;

[Context("Bootstrapper")]
internal class SteamWorkshopModDownloadingConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<IModsProvider>().To<SteamWorkshopModsProvider>().AsSingleton();
	}
}
