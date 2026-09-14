using Bindito.Core;

namespace Timberborn.MainMenuSceneLoading;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class MainMenuSceneLoadingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MainMenuSceneLoader>().AsSingleton();
	}
}
