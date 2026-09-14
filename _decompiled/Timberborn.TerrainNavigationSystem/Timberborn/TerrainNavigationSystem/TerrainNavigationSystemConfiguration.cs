using Bindito.Core;

namespace Timberborn.TerrainNavigationSystem;

[Context("Game")]
internal class TerrainNavigationSystemConfiguration : Configurator
{
	protected override void Configure()
	{
		Bind<TerrainNavMeshUpdater>().AsSingleton();
	}
}
