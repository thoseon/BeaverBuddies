using System;
using System.IO;
using Bindito.Core;
using Timberborn.ErrorReporting;
using Timberborn.MapSystem;
using UnityEngine;

namespace Timberborn.MapEditorScene;

public class MapEditorSceneExceptionStateSaver : MonoBehaviour
{
	private MapSaver _mapSaver;

	[Inject]
	public void InjectDependencies(MapSaver mapSaver)
	{
		_mapSaver = mapSaver;
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
		Debug.Log("Creating an exception map save");
		ErrorReporter.ExceptionSave = CreateExceptionSave();
	}

	private byte[] CreateExceptionSave()
	{
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			_mapSaver.Save(memoryStream);
			return memoryStream.ToArray();
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Failed to create an exception map save due to exception: {arg}");
		}
		return null;
	}
}
