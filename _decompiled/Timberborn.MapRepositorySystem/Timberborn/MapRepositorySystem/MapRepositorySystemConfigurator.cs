using Bindito.Core;

namespace Timberborn.MapRepositorySystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class MapRepositorySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapRepository>().AsSingleton();
		Bind<MapDeserializer>().AsSingleton();
	}
}
