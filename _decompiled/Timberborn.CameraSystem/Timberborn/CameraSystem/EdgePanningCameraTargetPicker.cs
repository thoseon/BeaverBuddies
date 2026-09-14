using Timberborn.BlueprintSystem;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CameraSystem;

public class EdgePanningCameraTargetPicker : ILoadableSingleton
{
	private readonly CameraMovementInput _cameraMovementInput;

	private readonly InputSettings _inputSettings;

	private readonly ISpecService _specService;

	private EdgePanningCameraTargetPickerSpec _edgePanningCameraTargetPickerSpec;

	private bool _suspended;

	private float _minBaseSpeed;

	private float _maxBaseSpeed;

	private float _fastMovementSpeedBonus;

	private float Speed => BaseSpeed + SpeedBonus;

	private float BaseSpeed => _minBaseSpeed + (_maxBaseSpeed - _minBaseSpeed) * _inputSettings.EdgePanCameraSpeed;

	private float SpeedBonus
	{
		get
		{
			if (!_cameraMovementInput.MoveCameraFast)
			{
				return 0f;
			}
			return _fastMovementSpeedBonus;
		}
	}

	public EdgePanningCameraTargetPicker(CameraMovementInput cameraMovementInput, InputSettings inputSettings, ISpecService specService)
	{
		_cameraMovementInput = cameraMovementInput;
		_inputSettings = inputSettings;
		_specService = specService;
	}

	public void Load()
	{
		EdgePanningCameraTargetPickerSpec singleSpec = _specService.GetSingleSpec<EdgePanningCameraTargetPickerSpec>();
		_minBaseSpeed = singleSpec.MinBaseSpeed;
		_maxBaseSpeed = singleSpec.MaxBaseSpeed;
		_fastMovementSpeedBonus = singleSpec.FastMovementSpeedBonus;
	}

	public Vector3 CameraPositionDelta(float zoomSpeedScale)
	{
		if (!_suspended)
		{
			Vector3 movementDirection = MovementDirection();
			return CameraPositionDelta(zoomSpeedScale, movementDirection);
		}
		return Vector3.zero;
	}

	public void Suspend()
	{
		_suspended = true;
	}

	private Vector3 MovementDirection()
	{
		ScreenEdges mouseScreenEdges = _cameraMovementInput.GetMouseScreenEdges();
		Vector3 zero = Vector3.zero;
		if (mouseScreenEdges.HasFlag(ScreenEdges.Down))
		{
			zero += Vector3.back;
		}
		if (mouseScreenEdges.HasFlag(ScreenEdges.Left))
		{
			zero += Vector3.left;
		}
		if (mouseScreenEdges.HasFlag(ScreenEdges.Up))
		{
			zero += Vector3.forward;
		}
		if (mouseScreenEdges.HasFlag(ScreenEdges.Right))
		{
			zero += Vector3.right;
		}
		return zero;
	}

	private Vector3 CameraPositionDelta(float zoomSpeedScale, Vector3 movementDirection)
	{
		if (!movementDirection.Equals(Vector3.zero))
		{
			float num = zoomSpeedScale * Speed * CappedTime.CappedUnscaledDeltaTime();
			return movementDirection * num;
		}
		return Vector3.zero;
	}
}
