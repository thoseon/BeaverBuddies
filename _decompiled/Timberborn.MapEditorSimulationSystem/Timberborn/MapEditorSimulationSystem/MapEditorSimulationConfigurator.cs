using Bindito.Core;

namespace Timberborn.MapEditorSimulationSystem;

[Context("MapEditor")]
internal class MapEditorSimulationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapEditorSimulation>().AsSingleton();
	}
}
