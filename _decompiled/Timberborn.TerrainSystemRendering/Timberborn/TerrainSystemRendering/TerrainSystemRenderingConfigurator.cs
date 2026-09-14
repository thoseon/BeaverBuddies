using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.TerrainSystemRendering;

[Context("Game")]
[Context("MapEditor")]
internal class TerrainSystemRenderingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TerrainBlockRandomizer>().AsSingleton();
		Bind<TerrainTopMeshService>().AsSingleton();
		Bind<TerrainMeshManager>().AsSingleton();
		Bind<TerrainBlockRepository>().AsSingleton();
		Bind<TerrainMaterialMap>().AsSingleton();
		Bind<SurfaceBlockCollectionFactory>().AsSingleton();
		Bind<TerrainLayerSliceUpdater>().AsSingleton();
		Bind<TerrainHighlightingService>().AsSingleton();
		MultiBind<IDevModule>().To<TerrainSystemRenderingDevModule>().AsSingleton();
	}
}
