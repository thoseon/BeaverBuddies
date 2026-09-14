using Bindito.Core;

namespace Timberborn.ThumbnailCapturing;

[Context("Game")]
[Context("MapEditor")]
internal class ThumbnailCapturingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ThumbnailCamera>().AsSingleton();
		Bind<ThumbnailRenderer>().AsSingleton();
		Bind<ThumbnailSaveEntryWriter>().AsSingleton();
	}
}
