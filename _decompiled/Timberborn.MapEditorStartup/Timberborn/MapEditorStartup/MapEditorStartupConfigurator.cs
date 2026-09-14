using Bindito.Core;

namespace Timberborn.MapEditorStartup;

[Context("MapEditor")]
internal class MapEditorStartupConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapEditorInitializer>().AsSingleton();
	}
}
