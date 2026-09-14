using Bindito.Core;

namespace Timberborn.MapEditorPersistenceUI;

[Context("MapEditor")]
internal class MapEditorPersistenceUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapPersistenceController>().AsSingleton();
		Bind<MapSaverLoader>().AsSingleton();
		Bind<SaveMapBox>().AsSingleton();
	}
}
