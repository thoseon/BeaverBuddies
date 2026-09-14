using Bindito.Core;

namespace Timberborn.SaveThumbnail;

[Context("MainMenu")]
[Context("Game")]
internal class SaveThumbnailConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SaveThumbnailConfiguration>().AsSingleton();
		Bind<SaveThumbnailSaveEntryReader>().AsSingleton();
	}
}
