using System;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.QuickNotificationSystem;
using Timberborn.SerializationSystem;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.CameraSystem;

internal class CameraStateRestorer : ISaveableSingleton, ILoadableSingleton, IInputProcessor
{
	private static readonly string SaveCameraKey = "SaveCamera";

	private static readonly string RestoreCameraKey = "RestoreCamera";

	private static readonly string SaveCameraToClipboardKey = "SaveCameraToClipboard";

	private static readonly string RestoreCameraFromClipboardKey = "RestoreCameraFromClipboard";

	private static readonly string ClipboardStateKey = "CameraState";

	private static readonly SingletonKey CameraStateRestorerKey = new SingletonKey("CameraStateRestorer");

	private static readonly PropertyKey<CameraState> SavedCameraStateKey = new PropertyKey<CameraState>("SavedCameraState");

	private static readonly string CameraStateSavedLocKey = "Camera.StateSaved";

	private readonly ISingletonLoader _singletonLoader;

	private readonly InputService _inputService;

	private readonly CameraService _cameraService;

	private readonly QuickNotificationService _quickNotificationService;

	private readonly ILoc _loc;

	private readonly CameraStateSerializer _cameraStateSerializer;

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	private readonly MapEditorMode _mapEditorMode;

	private CameraState? _savedCameraState;

	public CameraStateRestorer(ISingletonLoader singletonLoader, InputService inputService, CameraService cameraService, QuickNotificationService quickNotificationService, ILoc loc, CameraStateSerializer cameraStateSerializer, SerializedObjectReaderWriter serializedObjectReaderWriter, MapEditorMode mapEditorMode)
	{
		_singletonLoader = singletonLoader;
		_inputService = inputService;
		_cameraService = cameraService;
		_quickNotificationService = quickNotificationService;
		_loc = loc;
		_cameraStateSerializer = cameraStateSerializer;
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
		_mapEditorMode = mapEditorMode;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(CameraStateRestorerKey, out var objectLoader))
		{
			_savedCameraState = objectLoader.Get(SavedCameraStateKey, _cameraStateSerializer);
		}
		_inputService.AddInputProcessor(this);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_savedCameraState.HasValue && !_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(CameraStateRestorerKey).Set(SavedCameraStateKey, _savedCameraState.Value, _cameraStateSerializer);
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(SaveCameraKey))
		{
			SaveCameraState();
			return true;
		}
		if (_inputService.IsKeyDown(SaveCameraToClipboardKey))
		{
			SaveCameraStateToClipboard();
			return true;
		}
		if (_inputService.IsKeyDown(RestoreCameraKey))
		{
			LoadCameraState();
			return true;
		}
		if (_inputService.IsKeyDown(RestoreCameraFromClipboardKey))
		{
			LoadCameraStateFromClipboard();
			return true;
		}
		return false;
	}

	public void SaveCameraState()
	{
		_savedCameraState = _cameraService.GetCurrentState();
		_quickNotificationService.SendNotification(_loc.T(CameraStateSavedLocKey));
	}

	public void LoadCameraState()
	{
		if (_savedCameraState.HasValue)
		{
			_cameraService.RestoreState(_savedCameraState.Value);
		}
	}

	public void SaveCameraStateToClipboard()
	{
		CameraState currentState = _cameraService.GetCurrentState();
		ValueSaver valueSaver = new ValueSaver();
		_cameraStateSerializer.Serialize(currentState, valueSaver);
		SerializedObject serializedObject = new SerializedObject();
		serializedObject.Set(ClipboardStateKey, valueSaver.Value);
		GUIUtility.systemCopyBuffer = _serializedObjectReaderWriter.WriteJson(serializedObject);
		_quickNotificationService.SendNotification(_loc.T(CameraStateSavedLocKey));
	}

	public void LoadCameraStateFromClipboard()
	{
		try
		{
			ObjectLoader objectLoader = new ObjectLoader(_serializedObjectReaderWriter.ReadJson(GUIUtility.systemCopyBuffer));
			PropertyKey<CameraState> key = new PropertyKey<CameraState>(ClipboardStateKey);
			CameraState cameraState = objectLoader.Get(key, _cameraStateSerializer);
			_cameraService.RestoreState(cameraState);
		}
		catch (Exception)
		{
			_quickNotificationService.SendNotification("Clipboard does not contain a valid camera state.");
		}
	}
}
