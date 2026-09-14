using Bindito.Core;

namespace Timberborn.MapThumbnail;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class MapThumbnailConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapThumbnailConfiguration>().AsSingleton();
		Bind<MapThumbnailSaveEntryReader>().AsSingleton();
		Bind<MapThumbnailCache>().AsSingleton();
	}
}
