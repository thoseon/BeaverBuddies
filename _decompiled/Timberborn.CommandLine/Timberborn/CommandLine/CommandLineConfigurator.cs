using Bindito.Core;

namespace Timberborn.CommandLine;

[Context("Bootstrapper")]
internal class CommandLineConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ICommandLineArguments>().ToProvider(CommandLineArguments.CreateWithCommandLineArgs).AsSingleton().AsExported();
	}
}
