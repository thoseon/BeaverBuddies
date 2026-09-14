using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CameraSystem;

internal class DraggingCameraTargetPicker : ILoadableSingleton
{
	private readonly InputService _inputService;

	private readonly CameraActionMarker _cameraActionMarker;

	private readonly EventBus _eventBus;

	private readonly ISpecService _specService;

	private Vector2? _startingMousePosition;

	private float _movementSpeed;

	public DraggingCameraTargetPicker(InputService inputService, CameraActionMarker cameraActionMarker, EventBus eventBus, ISpecService specService)
	{
		_inputService = inputService;
		_cameraActionMarker = cameraActionMarker;
		_eventBus = eventBus;
		_specService = specService;
	}

	public void Load()
	{
		DraggingCameraTargetPickerSpec singleSpec = _specService.GetSingleSpec<DraggingCameraTargetPickerSpec>();
		_movementSpeed = singleSpec.MovementSpeed;
		_eventBus.Register(this);
	}

	public Vector3 CameraPositionDelta()
	{
		if (!_startingMousePosition.HasValue)
		{
			if (_inputService.MoveButtonHeld)
			{
				StartDragging();
			}
		}
		else
		{
			if (_inputService.MoveButtonHeld)
			{
				return CameraPositionDelta(_startingMousePosition.Value);
			}
			StopDragging();
		}
		return Vector3.zero;
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		if (_startingMousePosition.HasValue)
		{
			StopDragging();
		}
	}

	private Vector3 CameraPositionDelta(Vector2 startingMousePosition)
	{
		Vector2 vector = _inputService.MousePositionNdc - startingMousePosition;
		Vector3 normalized = new Vector3(vector.x, 0f, vector.y).normalized;
		float num = vector.magnitude * _movementSpeed * CappedTime.CappedUnscaledDeltaTime();
		return normalized * num;
	}

	private void StartDragging()
	{
		_startingMousePosition = _inputService.MousePositionNdc;
		_cameraActionMarker.ShowMarker(_startingMousePosition.Value);
	}

	private void StopDragging()
	{
		_startingMousePosition = null;
		_cameraActionMarker.Hide();
	}
}
