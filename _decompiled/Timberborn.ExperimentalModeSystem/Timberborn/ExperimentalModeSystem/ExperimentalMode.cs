using Timberborn.CommandLine;
using Timberborn.SingletonSystem;

namespace Timberborn.ExperimentalModeSystem;

public class ExperimentalMode : ILoadableSingleton
{
	private static readonly string ExperimentalModeKey = "experimental";

	private readonly ICommandLineArguments _commandLineArguments;

	public bool IsExperimental { get; private set; }

	public ExperimentalMode(ICommandLineArguments commandLineArguments)
	{
		_commandLineArguments = commandLineArguments;
	}

	public void Load()
	{
		if (_commandLineArguments.Has(ExperimentalModeKey))
		{
			EnableExperimentalMode();
		}
	}

	private void EnableExperimentalMode()
	{
		IsExperimental = true;
	}
}
