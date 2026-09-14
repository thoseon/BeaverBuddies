using Bindito.Core;

namespace Timberborn.TerrainUndoSystem;

[Context("MapEditor")]
internal class TerrainUndoSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TerrainUndoableRegistrar>().AsSingleton();
	}
}
