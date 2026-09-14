using Bindito.Core;

namespace Timberborn.MapItemsUI;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class MapItemsUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapItemProvider>().AsSingleton();
		Bind<OfficialMapItemFactory>().AsSingleton();
		Bind<UserMapItemFactory>().AsSingleton();
		Bind<MapItemElementFactory>().AsSingleton();
		Bind<MapItemFactionIconFactory>().AsSingleton();
	}
}
