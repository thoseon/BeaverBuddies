using Timberborn.Debugging;
using Timberborn.QuickNotificationSystem;
using UnityEngine;

namespace Timberborn.CameraSystem;

internal class CameraSystemDevModule : IDevModule
{
	private static readonly string SaveCameraKey = "SaveCamera";

	private static readonly string RestoreCameraKey = "RestoreCamera";

	private static readonly string SaveCameraToClipboardKey = "SaveCameraToClipboard";

	private static readonly string RestoreCameraFromClipboardKey = "RestoreCameraFromClipboard";

	private static readonly float FieldOfViewStep = 2f;

	private static readonly float MoveTargetUpDownStep = 1f;

	private static readonly float NearClipPlaneMultiplier = 1.2f;

	private readonly CameraService _cameraService;

	private readonly CameraStateRestorer _cameraStateRestorer;

	private readonly QuickNotificationService _quickNotificationService;

	public CameraSystemDevModule(CameraService cameraService, CameraStateRestorer cameraStateRestorer, QuickNotificationService quickNotificationService)
	{
		_cameraService = cameraService;
		_cameraStateRestorer = cameraStateRestorer;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.CreateBindable("Camera state: Save", SaveCameraKey, delegate
		{
			_cameraStateRestorer.SaveCameraState();
		})).AddMethod(DevMethod.CreateBindable("Camera state: Restore", RestoreCameraKey, _cameraStateRestorer.LoadCameraState)).AddMethod(DevMethod.CreateBindable("Camera state: Copy", SaveCameraToClipboardKey, _cameraStateRestorer.SaveCameraStateToClipboard))
			.AddMethod(DevMethod.CreateBindable("Camera state: Paste", RestoreCameraFromClipboardKey, _cameraStateRestorer.LoadCameraStateFromClipboard))
			.AddMethod(DevMethod.Create("Camera: Free mode", ToggleFreeMode))
			.AddMethod(DevMethod.Create("Camera: FOV +", IncreaseFieldOfView))
			.AddMethod(DevMethod.Create("Camera: FOV -", DecreaseFieldOfView))
			.AddMethod(DevMethod.Create("Camera: Move target up", MoveTargetUp))
			.AddMethod(DevMethod.Create("Camera: Move target down", MoveTargetDown))
			.AddMethod(DevMethod.Create("Camera: Move clip plane nearer", MoveClipPlaneNearer))
			.AddMethod(DevMethod.Create("Camera: Move clip plane farther", MoveClipPlaneFarther))
			.Build();
	}

	private void IncreaseFieldOfView()
	{
		ModifyFieldOfView(FieldOfViewStep);
	}

	private void DecreaseFieldOfView()
	{
		ModifyFieldOfView(0f - FieldOfViewStep);
	}

	private void MoveTargetUp()
	{
		MoveTargetVertically(MoveTargetUpDownStep);
	}

	private void MoveTargetDown()
	{
		MoveTargetVertically(0f - MoveTargetUpDownStep);
	}

	private void MoveClipPlaneNearer()
	{
		MoveClipPlane(1f / NearClipPlaneMultiplier);
	}

	private void MoveClipPlaneFarther()
	{
		MoveClipPlane(NearClipPlaneMultiplier);
	}

	private void ToggleFreeMode()
	{
		_cameraService.FreeMode = !_cameraService.FreeMode;
		_quickNotificationService.SendNotification("Free camera " + (_cameraService.FreeMode ? "ON" : "OFF"));
	}

	private void ModifyFieldOfView(float delta)
	{
		_cameraService.FieldOfView += delta;
		_quickNotificationService.SendNotification($"Field of view: {_cameraService.FieldOfView}");
	}

	private void MoveTargetVertically(float delta)
	{
		_cameraService.MoveCameraBy(new Vector3(0f, delta, 0f));
		_quickNotificationService.SendNotification($"Target height: {_cameraService.Target.y:0.0}");
	}

	private void MoveClipPlane(float multiplier)
	{
		_cameraService.NearClipPlane *= multiplier;
		_quickNotificationService.SendNotification($"Clip plane: {_cameraService.NearClipPlane:0.0}");
	}
}
