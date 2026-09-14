using Bindito.Core;

namespace Timberborn.TitleScreenUI;

[Context("MainMenu")]
internal class TitleScreenUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TitleScreen>().AsSingleton();
		Bind<TitleScreenFooter>().AsSingleton();
		Bind<ChangeLanguageButtonInitializer>().AsSingleton();
		Bind<MacOsRosettaWarningInitializer>().AsSingleton();
	}
}
