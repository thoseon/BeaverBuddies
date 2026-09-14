using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Timberborn.ErrorReporting;

public static class ExceptionListener
{
	internal static bool AnyUncaughtException;

	private static bool ContinueOnErrors => false;

	public static event EventHandler FirstUncaughtException;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Initialize()
	{
		Application.logMessageReceived += OnLog;
	}

	private static void OnLog(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			OnUncaughtException(logString, stackTrace);
		}
	}

	private static void OnUncaughtException(string logString, string stackTrace)
	{
		if (!AnyUncaughtException)
		{
			AnyUncaughtException = true;
			Application.logMessageReceived -= OnLog;
			string text = $"First uncaught exception at {DateTime.Now:u}";
			Debug.LogError(Application.isEditor ? (text + " (click for details)\n\n" + logString + "\n\n" + stackTrace + "\n\n") : (text + "\n\n" + logString + "\n\n" + stackTrace + "\n\n"));
			RememberError(logString, stackTrace);
			if (!ContinueOnErrors)
			{
				StopAllRootObjects();
			}
			InvokeListeners();
			CrashSceneLoader.LoadCrashSceneIfEnabled();
		}
	}

	private static void RememberError(string logString, string stackTrace)
	{
		ErrorReporter.LogString = logString;
		ErrorReporter.StackTrace = stackTrace;
	}

	private static void InvokeListeners()
	{
		try
		{
			FirstUncaughtException?.Invoke(null, EventArgs.Empty);
		}
		catch (Exception arg)
		{
			Debug.LogError($"Exception while invoking listeners: {arg}");
		}
	}

	private static void StopAllRootObjects()
	{
		Debug.Log("Stopping all root objects in active scene");
		try
		{
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				rootGameObjects[i].SetActive(value: false);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"Exception while stopping objects: {arg}");
		}
	}
}
