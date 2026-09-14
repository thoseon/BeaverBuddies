namespace Timberborn.CommandLine;

public interface ICommandLineArguments
{
	bool Has(string key);

	int GetInt(string key);

	string GetString(string key);
}
