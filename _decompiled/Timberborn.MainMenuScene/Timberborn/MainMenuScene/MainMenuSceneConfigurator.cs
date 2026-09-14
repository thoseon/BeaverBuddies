using Bindito.Core;

namespace Timberborn.MainMenuScene;

[Context("MainMenu")]
internal class MainMenuSceneConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AutoStarter>().AsSingleton();
		Bind<PlayerDataLoader>().AsSingleton();
		Bind<InitialLanguageChooser>().AsSingleton();
		Bind<MacOSPermissionsChecker>().AsSingleton();
		Bind<WelcomeScreenBox>().AsSingleton();
		Bind<MainMenuInitializer>().AsSingleton();
	}
}
