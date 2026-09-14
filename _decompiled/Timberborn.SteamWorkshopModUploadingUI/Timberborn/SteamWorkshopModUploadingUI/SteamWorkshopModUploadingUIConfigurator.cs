using Bindito.Core;

namespace Timberborn.SteamWorkshopModUploadingUI;

[Context("MainMenu")]
internal class SteamWorkshopModUploadingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamWorkshopModUploader>().AsSingleton();
		Bind<SteamWorkshopUploadableModFactory>().AsSingleton();
	}
}
