using System;
using System.Collections.Generic;
using System.Linq;

namespace Timberborn.CommandLine;

public class CommandLineArguments : ICommandLineArguments
{
	private readonly string[] _arguments;

	public CommandLineArguments(IEnumerable<string> arguments)
	{
		_arguments = arguments.ToArray();
	}

	public static CommandLineArguments CreateWithCommandLineArgs()
	{
		return new CommandLineArguments(Environment.GetCommandLineArgs());
	}

	public bool Has(string key)
	{
		return IndexOfKey(key) >= _arguments.GetLowerBound(0);
	}

	public int GetInt(string key)
	{
		return int.Parse(GetString(key));
	}

	public string GetString(string key)
	{
		if (!Has(key))
		{
			throw new ArgumentException("Argument " + key + " was not found.");
		}
		int num = IndexOfKey(key) + 1;
		return _arguments[num];
	}

	private int IndexOfKey(string key)
	{
		return Array.IndexOf(_arguments, "-" + key);
	}
}
