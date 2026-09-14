using Bindito.Core;

namespace Timberborn.MapSystem;

[Context("Game")]
[Context("MapEditor")]
internal class MapSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapLoader>().AsSingleton();
		Bind<MapSaver>().AsSingleton();
	}
}
