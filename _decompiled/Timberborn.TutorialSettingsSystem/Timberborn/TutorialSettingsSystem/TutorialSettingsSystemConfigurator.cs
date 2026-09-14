using Bindito.Core;

namespace Timberborn.TutorialSettingsSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class TutorialSettingsSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TutorialSettings>().AsSingleton();
	}
}
