using Bindito.Core;
using Timberborn.SaveSystem;
using Timberborn.ThumbnailCapturing;

namespace Timberborn.MapThumbnailCapturing;

[Context("MapEditor")]
internal class MapThumbnailCapturingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ThumbnailCameraDefaultPositionProvider>().AsSingleton();
		Bind<CameraConfigurationSerializer>().AsSingleton();
		Bind<MapThumbnailCameraMover>().AsSingleton();
		Bind<IThumbnailRenderTextureProvider>().To<MapThumbnailRenderTextureProvider>().AsSingleton();
		MultiBind<ISaveEntryWriter>().To<MapThumbnailSaveEntryWriter>().AsSingleton();
		MultiBind<IThumbnailRenderingListener>().To<ShadowThumbnailRenderingListener>().AsSingleton();
		MultiBind<IThumbnailRenderingListener>().To<SunThumbnailRenderingListener>().AsSingleton();
		MultiBind<IThumbnailRenderingListener>().To<StartingLocationThumbnailRenderingListener>().AsSingleton();
		MultiBind<IThumbnailRenderingListener>().To<WaterThumbnailRenderingListener>().AsSingleton();
		MultiBind<IThumbnailRenderingListener>().To<SelectionThumbnailRenderingListener>().AsSingleton();
	}
}
