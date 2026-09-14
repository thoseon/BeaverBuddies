using Bindito.Core;

namespace Timberborn.MapEditorSimulationSystemUI;

[Context("MapEditor")]
internal class MapEditorSimulationUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapEditorSimulationPanel>().AsSingleton();
	}
}
