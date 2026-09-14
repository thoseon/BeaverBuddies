using Bindito.Core;

namespace Timberborn.TerrainSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class TerrainSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TerrainDebuggingPanel>().AsSingleton();
	}
}
