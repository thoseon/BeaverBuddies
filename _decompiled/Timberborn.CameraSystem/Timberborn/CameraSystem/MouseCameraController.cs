using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CameraSystem;

internal class MouseCameraController : IInputProcessor, ILoadableSingleton
{
	private readonly InputService _inputService;

	private readonly InputSettings _inputSettings;

	private readonly CameraActionMarker _cameraActionMarker;

	private readonly CameraService _cameraService;

	private readonly EventBus _eventBus;

	private readonly DraggingCameraTargetPicker _draggingCameraTargetPicker;

	private readonly GrabbingCameraTargetPicker _grabbingCameraTargetPicker;

	private readonly EdgePanningCameraTargetPicker _edgePanningCameraTargetPicker;

	private readonly ISpecService _specService;

	private bool _rotating;

	private float _rotationDistanceAccumulator;

	private MouseCameraControllerSpec _mouseCameraControllerSpec;

	public MouseCameraController(InputService inputService, InputSettings inputSettings, CameraActionMarker cameraActionMarker, CameraService cameraService, EventBus eventBus, DraggingCameraTargetPicker draggingCameraTargetPicker, GrabbingCameraTargetPicker grabbingCameraTargetPicker, EdgePanningCameraTargetPicker edgePanningCameraTargetPicker, ISpecService specService)
	{
		_inputService = inputService;
		_inputSettings = inputSettings;
		_cameraActionMarker = cameraActionMarker;
		_cameraService = cameraService;
		_eventBus = eventBus;
		_draggingCameraTargetPicker = draggingCameraTargetPicker;
		_grabbingCameraTargetPicker = grabbingCameraTargetPicker;
		_edgePanningCameraTargetPicker = edgePanningCameraTargetPicker;
		_specService = specService;
	}

	public void Load()
	{
		_mouseCameraControllerSpec = _specService.GetSingleSpec<MouseCameraControllerSpec>();
		_inputService.AddInputProcessor(this);
		_eventBus.Register(this);
	}

	public bool ProcessInput()
	{
		ScrollWheelUpdate();
		MovementUpdate();
		RotationUpdate();
		return false;
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		if (_rotating)
		{
			StopRotatingCamera();
		}
	}

	private void ScrollWheelUpdate()
	{
		if (!_inputService.MouseOverUI)
		{
			float mouseZoom = _inputService.MouseZoom;
			_cameraService.ModifyZoomLevel(mouseZoom);
		}
	}

	private void MovementUpdate()
	{
		if (_inputSettings.DragCamera)
		{
			MoveCameraByDragging();
		}
		else
		{
			MoveCameraByGrabbingTerrain();
		}
		if (_inputSettings.EdgePanCamera)
		{
			MoveCameraByEdgePanning();
		}
	}

	private void MoveCameraByDragging()
	{
		Vector3 delta = _draggingCameraTargetPicker.CameraPositionDelta();
		_cameraService.MoveCameraBy(delta);
	}

	private void MoveCameraByGrabbingTerrain()
	{
		Vector3 point = _grabbingCameraTargetPicker.PickCameraTarget();
		_cameraService.MoveTargetTo(point);
	}

	private void MoveCameraByEdgePanning()
	{
		float zoomSpeedScale = _cameraService.ZoomSpeedScale;
		Vector3 delta = _edgePanningCameraTargetPicker.CameraPositionDelta(zoomSpeedScale);
		_cameraService.MoveCameraBy(delta);
	}

	private void RotationUpdate()
	{
		if (_inputService.RotateButtonHeld && !_inputService.MoveButtonHeld)
		{
			if (!_rotating && _inputService.MouseXYAxes != Vector2.zero)
			{
				StartRotatingCamera();
			}
			if (_rotating)
			{
				RotateCamera();
			}
		}
		else if (_rotating)
		{
			StopRotatingCamera();
		}
	}

	private void StartRotatingCamera()
	{
		_rotating = true;
		_rotationDistanceAccumulator = 0f;
		_inputService.LockCursor();
		_inputService.HideCursor();
	}

	private void StopRotatingCamera()
	{
		_rotating = false;
		_rotationDistanceAccumulator = 0f;
		_cameraActionMarker.Hide();
		_inputService.UnlockCursor();
		_inputService.ShowCursor();
	}

	private void RotateCamera()
	{
		Vector2 mouseXYAxes = _inputService.MouseXYAxes;
		_rotationDistanceAccumulator += mouseXYAxes.magnitude;
		if (_rotationDistanceAccumulator > _mouseCameraControllerSpec.RmbRotationMinDistance || _inputService.RotateButtonLongHeld)
		{
			Vector2 vector = _mouseCameraControllerSpec.RmbRotationSpeed * mouseXYAxes * _inputSettings.MouseCameraRotationSpeed;
			_cameraService.ModifyHorizontalAngle(vector.x);
			_cameraService.ModifyVerticalAngle(0f - vector.y);
		}
	}
}
