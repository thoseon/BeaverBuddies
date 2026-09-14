using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace Timberborn.Console;

internal static class ConsoleLogListener
{
	private static readonly int MaxLogs = 1000;

	private static readonly Queue<Log> Logs = new Queue<Log>();

	internal static bool AnyWarningOrError;

	public static event EventHandler<Log> OnLogReceived;

	public static event EventHandler<Log> OnFirstWarningOrErrorReceived;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	public static void Initialize()
	{
		Application.logMessageReceived += OnLogMessageReceived;
	}

	public static ImmutableArray<Log> GetAllLogs()
	{
		return Logs.ToImmutableArray();
	}

	private static void OnLogMessageReceived(string message, string stacktrace, LogType type)
	{
		try
		{
			Log log = new Log(message, type);
			Logs.Enqueue(log);
			Trim();
			OnLogReceived?.Invoke(null, log);
			bool flag = ((type == LogType.Error || type == LogType.Warning) ? true : false);
			if (flag && !AnyWarningOrError)
			{
				AnyWarningOrError = true;
				OnFirstWarningOrErrorReceived?.Invoke(null, log);
			}
		}
		catch (Exception arg)
		{
			Application.logMessageReceived -= OnLogMessageReceived;
			Debug.LogError($"Exception while processing a log message: {arg}");
		}
	}

	private static void Trim()
	{
		while (Logs.Count > MaxLogs)
		{
			Logs.Dequeue();
		}
	}
}
