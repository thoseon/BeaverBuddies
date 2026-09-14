using System;
using Timberborn.BlueprintSystem;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CameraSystem;

internal class KeyboardCameraController : IInputProcessor, ILoadableSingleton
{
	private static readonly string ZoomInKey = "ZoomIn";

	private static readonly string ZoomOutKey = "ZoomOut";

	private readonly InputService _inputService;

	private readonly CameraService _cameraService;

	private readonly CameraMovementInput _cameraMovementInput;

	private readonly InputSettings _inputSettings;

	private readonly ISpecService _specService;

	private KeyboardCameraControllerSpec _keyboardCameraControllerSpec;

	private float _remainingHorizontalJumpAngle;

	private float MovementSpeed => (_inputSettings.KeyboardCameraMovementSpeed * 50f + 1f) * (float)((!_cameraMovementInput.MoveCameraFast) ? 1 : 2);

	private float RotationSpeed => (_inputSettings.KeyboardCameraRotationSpeed * 175f + 1f) * (float)((!_cameraMovementInput.MoveCameraFast) ? 1 : 2);

	private float ZoomSpeed => _keyboardCameraControllerSpec.BaseZoomSpeed * _inputSettings.KeyboardCameraZoomSpeed;

	public KeyboardCameraController(InputService inputService, CameraService cameraService, CameraMovementInput cameraMovementInput, InputSettings inputSettings, ISpecService specService)
	{
		_inputService = inputService;
		_cameraService = cameraService;
		_cameraMovementInput = cameraMovementInput;
		_inputSettings = inputSettings;
		_specService = specService;
	}

	public void Load()
	{
		_keyboardCameraControllerSpec = _specService.GetSingleSpec<KeyboardCameraControllerSpec>();
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		MovementUpdate();
		RotationUpdate();
		ZoomUpdate();
		return false;
	}

	private void MovementUpdate()
	{
		Vector2 cameraMovementAxes = _cameraMovementInput.CameraMovementAxes;
		Vector3 normalized = new Vector3(cameraMovementAxes.x, 0f, cameraMovementAxes.y).normalized;
		Vector3 delta = MovementSpeed * _cameraService.ZoomSpeedScale * CappedTime.CappedUnscaledDeltaTime() * normalized;
		_cameraService.MoveCameraBy(delta);
	}

	private void RotationUpdate()
	{
		float rotationSpeed = CappedTime.CappedUnscaledDeltaTime() * RotationSpeed;
		SmoothRotationUpdate(rotationSpeed);
		JumpRotationUpdate(rotationSpeed);
	}

	private void SmoothRotationUpdate(float rotationSpeed)
	{
		Vector2 cameraRotationAxes = _cameraMovementInput.GetCameraRotationAxes();
		_cameraService.ModifyHorizontalAngle((0f - cameraRotationAxes.x) * rotationSpeed);
		_cameraService.ModifyVerticalAngle(cameraRotationAxes.y * rotationSpeed);
	}

	private void JumpRotationUpdate(float rotationSpeed)
	{
		_remainingHorizontalJumpAngle += KeyboardJumpRotationAngle();
		SmoothlyJumpHorizontally(rotationSpeed);
	}

	private float KeyboardJumpRotationAngle()
	{
		return _cameraMovementInput.GetCameraJumpRotationAxes().x * (float)_keyboardCameraControllerSpec.JumpRotationAngle;
	}

	private void SmoothlyJumpHorizontally(float rotationSpeed)
	{
		float num = (float)_keyboardCameraControllerSpec.JumpRotationSpeedInAnglePerUpdate * rotationSpeed;
		float num2 = ((Math.Abs(_remainingHorizontalJumpAngle) > num) ? (num * (float)Math.Sign(_remainingHorizontalJumpAngle)) : _remainingHorizontalJumpAngle);
		_remainingHorizontalJumpAngle -= num2;
		_cameraService.ModifyHorizontalAngle(0f - num2);
	}

	private void ZoomUpdate()
	{
		float num = ZoomSpeed * CappedTime.CappedUnscaledDeltaTime();
		if (_inputService.IsKeyHeld(ZoomInKey))
		{
			_cameraService.ModifyZoomLevel(num);
		}
		if (_inputService.IsKeyHeld(ZoomOutKey))
		{
			_cameraService.ModifyZoomLevel(0f - num);
		}
	}
}
