using Bindito.Core;
using Timberborn.SaveSystem;
using Timberborn.ThumbnailCapturing;

namespace Timberborn.SaveThumbnailCapturing;

[Context("Game")]
internal class SaveThumbnailCapturingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IThumbnailRenderTextureProvider>().To<SaveThumbnailRenderTextureProvider>().AsSingleton();
		MultiBind<ISaveEntryWriter>().To<SaveThumbnailSaveEntryWriter>().AsSingleton();
		MultiBind<IThumbnailRenderingListener>().To<SaveThumbnailRenderingListener>().AsSingleton();
	}
}
