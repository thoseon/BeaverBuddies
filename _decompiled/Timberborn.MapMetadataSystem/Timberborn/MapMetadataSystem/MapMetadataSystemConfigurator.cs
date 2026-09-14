using Bindito.Core;

namespace Timberborn.MapMetadataSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class MapMetadataSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapMetadataSerializer>().AsSingleton();
	}
}
