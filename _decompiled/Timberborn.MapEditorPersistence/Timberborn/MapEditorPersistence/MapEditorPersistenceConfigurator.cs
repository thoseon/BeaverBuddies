using Bindito.Core;

namespace Timberborn.MapEditorPersistence;

[Context("MapEditor")]
internal class MapEditorPersistenceConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapEditorMapLoader>().AsSingleton();
	}
}
