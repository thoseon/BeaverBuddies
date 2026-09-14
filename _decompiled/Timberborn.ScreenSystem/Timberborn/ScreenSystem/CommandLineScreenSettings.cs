using Timberborn.CommandLine;
using Timberborn.SingletonSystem;

namespace Timberborn.ScreenSystem;

internal class CommandLineScreenSettings : ILoadableSingleton
{
	private static readonly string UncappedCommandLineArgumentKey = "uncapped";

	private readonly ICommandLineArguments _commandLineArguments;

	public bool Uncapped { get; private set; }

	public CommandLineScreenSettings(ICommandLineArguments commandLineArguments)
	{
		_commandLineArguments = commandLineArguments;
	}

	public void Load()
	{
		Uncapped = _commandLineArguments.Has(UncappedCommandLineArgumentKey);
	}
}
