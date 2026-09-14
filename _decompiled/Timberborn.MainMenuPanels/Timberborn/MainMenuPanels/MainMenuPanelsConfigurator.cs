using Bindito.Core;

namespace Timberborn.MainMenuPanels;

[Context("MainMenu")]
internal class MainMenuPanelsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MainMenuPanel>().AsSingleton();
		Bind<CreditsBox>().AsSingleton();
		Bind<NewGameFactionPanel>().AsSingleton();
		Bind<NewGameMapPanel>().AsSingleton();
		Bind<NewGameModePanel>().AsSingleton();
		Bind<CustomNewGameModeController>().AsSingleton();
		Bind<MainMenuSoundController>().AsSingleton();
		Bind<TutorialToggleController>().AsSingleton();
	}
}
