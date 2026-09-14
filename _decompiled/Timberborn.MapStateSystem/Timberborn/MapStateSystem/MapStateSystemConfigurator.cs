using Bindito.Core;

namespace Timberborn.MapStateSystem;

[Context("Game")]
[Context("MapEditor")]
internal class MapStateSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapSize>().AsSingleton();
	}
}
