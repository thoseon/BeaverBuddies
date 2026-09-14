using Bindito.Core;

namespace Timberborn.Console;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class ConsoleConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IConsolePanel>().To<ConsolePanel>().AsSingleton();
	}
}
