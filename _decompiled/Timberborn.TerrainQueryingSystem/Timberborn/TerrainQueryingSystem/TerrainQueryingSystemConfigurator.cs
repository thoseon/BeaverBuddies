using Bindito.Core;

namespace Timberborn.TerrainQueryingSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TerrainQueryingSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TerrainPicker>().AsSingleton();
		Bind<TerrainAreaService>().AsSingleton();
	}
}
