using Bindito.Core;

namespace Timberborn.IntroSettingsSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class IntroSettingsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IntroSettings>().AsSingleton();
		Bind<IntroSettingsController>().AsSingleton();
	}
}
