using UnityEngine;

namespace Timberborn.Console;

internal readonly struct Log
{
	public string Message { get; }

	public LogType LogType { get; }

	public Log(string message, LogType logType)
	{
		Message = message;
		LogType = logType;
	}
}
