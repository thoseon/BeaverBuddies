using Bindito.Core;

namespace Timberborn.SoundSettingsSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class SoundSettingsSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Muter>().AsSingleton();
		Bind<SoundSettingsUpdater>().AsSingleton();
		Bind<SoundSettings>().AsSingleton();
	}
}
