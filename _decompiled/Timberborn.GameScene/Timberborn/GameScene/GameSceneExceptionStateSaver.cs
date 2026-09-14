using System;
using System.IO;
using Bindito.Core;
using Timberborn.ErrorReporting;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.GameScene;

public class GameSceneExceptionStateSaver : MonoBehaviour
{
	private GameSaver _gameSaver;

	private TickOnlyArrayService _tickOnlyArrayService;

	[Inject]
	public void InjectDependencies(GameSaver gameSaver, TickOnlyArrayService tickOnlyArrayService)
	{
		_gameSaver = gameSaver;
		_tickOnlyArrayService = tickOnlyArrayService;
	}

	public void Awake()
	{
		ExceptionListener.FirstUncaughtException += OnFirstUncaughtException;
	}

	public void OnDestroy()
	{
		ExceptionListener.FirstUncaughtException -= OnFirstUncaughtException;
	}

	private void OnFirstUncaughtException(object sender, EventArgs e)
	{
		Debug.Log("Creating an exception game save");
		ErrorReporter.ExceptionSave = CreateExceptionSave();
	}

	private byte[] CreateExceptionSave()
	{
		try
		{
			_tickOnlyArrayService.ForceAllowAccess();
			using MemoryStream memoryStream = new MemoryStream();
			_gameSaver.SaveWithoutFinishingTick(memoryStream);
			return memoryStream.ToArray();
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Failed to create an exception game save due to exception: {arg}");
		}
		return null;
	}
}
