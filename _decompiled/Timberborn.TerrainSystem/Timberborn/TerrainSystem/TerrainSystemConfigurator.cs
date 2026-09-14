using Bindito.Core;

namespace Timberborn.TerrainSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TerrainSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ITerrainService>().To<TerrainService>().AsSingleton();
		Bind<TerrainMap>().AsSingleton();
		Bind<ColumnTerrainMap>().AsSingleton();
		Bind<IThreadSafeColumnTerrainMap>().To<ThreadSafeColumnTerrainMap>().AsSingleton();
		Bind<TerrainCutout>().AsSingleton();
		Bind<CeilingRetriever>().AsSingleton();
	}
}
