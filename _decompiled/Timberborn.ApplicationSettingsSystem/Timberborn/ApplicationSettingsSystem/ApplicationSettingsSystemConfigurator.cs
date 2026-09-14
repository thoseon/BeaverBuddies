using Bindito.Core;

namespace Timberborn.ApplicationSettingsSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class ApplicationSettingsSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RunInBackgroundController>().AsSingleton();
	}
}
