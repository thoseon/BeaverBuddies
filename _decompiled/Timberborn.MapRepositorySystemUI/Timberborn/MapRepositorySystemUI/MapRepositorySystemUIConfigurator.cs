using Bindito.Core;

namespace Timberborn.MapRepositorySystemUI;

[Context("MainMenu")]
[Context("MapEditor")]
internal class MapRepositorySystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SelectedMapPanel>().AsTransient();
		Bind<MapSelection>().AsTransient();
		Bind<NewMapBox>().AsSingleton();
		Bind<LoadMapBox>().AsSingleton();
		Bind<MapValidator>().AsSingleton();
		Bind<MapVersionCompatibilityService>().AsSingleton();
		Bind<DevModeMapRepositoryChangeNotifier>().AsSingleton();
		Bind<MapDownloader>().AsSingleton();
		MultiBind<IMapLoadValidator>().To<MapVersionValidator>().AsSingleton();
	}
}
