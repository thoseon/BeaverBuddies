using Bindito.Core;

namespace Timberborn.ScreenSystem;

[Context("Bootstrapper")]
internal class ScreenSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ScreenSettings>().AsSingleton().AsExported();
		Bind<ScreenSettingsController>().AsSingleton();
		Bind<CommandLineScreenSettings>().AsSingleton();
		Bind<ScreenSettingsLogger>().AsSingleton();
	}
}
