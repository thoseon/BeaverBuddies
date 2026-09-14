using Bindito.Core;

namespace Timberborn.SettingsSystem;

[Context("Bootstrapper")]
internal class SettingsSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ISettings>().To<Settings>().AsSingleton().AsExported();
	}
}
