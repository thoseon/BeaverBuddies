using Bindito.Core;

namespace Timberborn.ThumbnailSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class ThumbnailSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ThumbnailSerializer>().AsSingleton();
	}
}
