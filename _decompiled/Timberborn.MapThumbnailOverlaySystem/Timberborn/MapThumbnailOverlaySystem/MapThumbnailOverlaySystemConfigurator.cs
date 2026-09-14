using Bindito.Core;
using Timberborn.SaveSystem;

namespace Timberborn.MapThumbnailOverlaySystem;

[Context("MapEditor")]
internal class MapThumbnailOverlaySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapThumbnailOverlay>().AsSingleton();
		Bind<MapThumbnailOverlaySerializer>().AsSingleton();
		MultiBind<ISaveEntryWriter>().To<MapThumbnailOverlaySaveEntryWriter>().AsSingleton();
	}
}
