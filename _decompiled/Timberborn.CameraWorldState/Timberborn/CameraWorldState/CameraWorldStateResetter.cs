using Timberborn.CameraSystem;
using Timberborn.Debugging;
using Timberborn.GridTraversing;
using Timberborn.InputSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainQueryingSystem;
using UnityEngine;

namespace Timberborn.CameraWorldState;

internal class CameraWorldStateResetter : IDevModule, IInputProcessor, ILoadableSingleton
{
	private static readonly string ResetCameraKey = "ResetCamera";

	private static readonly float DefaultZoomPerLevel = 0.1f;

	private static readonly float DefaultVerticalCameraAngle = 60f;

	private readonly CameraService _cameraService;

	private readonly CameraTargeter _cameraTargeter;

	private readonly TerrainPicker _terrainPicker;

	private readonly InputService _inputService;

	public CameraWorldStateResetter(CameraService cameraService, CameraTargeter cameraTargeter, TerrainPicker terrainPicker, InputService inputService)
	{
		_cameraService = cameraService;
		_cameraTargeter = cameraTargeter;
		_terrainPicker = terrainPicker;
		_inputService = inputService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.CreateBindable("Camera: Reset", ResetCameraKey, ResetCamera)).Build();
	}

	public void Load()
	{
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(ResetCameraKey))
		{
			ResetCamera();
			return true;
		}
		return false;
	}

	private void ResetCamera()
	{
		_cameraTargeter.StopFollowing();
		_cameraService.VerticalAngle = DefaultVerticalCameraAngle;
		Vector2 screenPoint = new Vector2(Screen.width, Screen.height) * 0.5f;
		Ray ray = _cameraService.ScreenPointToRayInGridSpace(screenPoint);
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		int num = (traversedCoordinates.HasValue ? traversedCoordinates.Value.Coordinates.z : 0);
		_cameraService.ZoomLevel = (float)num * DefaultZoomPerLevel;
	}
}
