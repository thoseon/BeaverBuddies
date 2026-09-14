using Timberborn.Coordinates;
using Timberborn.CoreUI;
using Timberborn.GridTraversing;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CameraSystem;

internal class GrabbingCameraTargetPicker : ILoadableSingleton
{
	private static readonly string CursorKey = "GrabbingCursor";

	private readonly InputService _inputService;

	private readonly ICameraAnchorPicker _cameraAnchorPicker;

	private readonly CameraService _cameraService;

	private readonly CursorService _cursorService;

	private readonly EventBus _eventBus;

	private float? _startingLevel;

	private Vector3 _startingTarget;

	private Vector2 _startingMousePosition;

	private bool _startedGrabbing;

	public GrabbingCameraTargetPicker(InputService inputService, ICameraAnchorPicker cameraAnchorPicker, CameraService cameraService, CursorService cursorService, EventBus eventBus)
	{
		_inputService = inputService;
		_cameraAnchorPicker = cameraAnchorPicker;
		_cameraService = cameraService;
		_cursorService = cursorService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public Vector3 PickCameraTarget()
	{
		if (_inputService.MoveButtonHeld)
		{
			if (!_startingLevel.HasValue)
			{
				StartGrabbing();
			}
			else
			{
				Vector3? vector = DeltaFromStartingTarget(_startingLevel.Value);
				if (vector.HasValue)
				{
					Vector3 valueOrDefault = vector.GetValueOrDefault();
					return _startingTarget + valueOrDefault;
				}
			}
		}
		else if (_startedGrabbing)
		{
			StopGrabbing();
		}
		return _cameraService.Target;
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		if (_startedGrabbing)
		{
			StopGrabbing();
		}
	}

	private void StartGrabbing()
	{
		_startedGrabbing = true;
		_cursorService.SetTemporaryCursor(CursorKey);
		_startingMousePosition = _inputService.MousePosition;
		Ray ray = _cameraService.ScreenPointToRayInGridSpace(_startingMousePosition);
		Vector3? vector = _cameraAnchorPicker.PickAnchorPoint(ray);
		if (vector.HasValue)
		{
			_startingLevel = CoordinateSystem.GridToWorld(vector.Value).y;
		}
		else
		{
			_startingLevel = 0f;
		}
		_startingTarget = _cameraService.Target;
	}

	private void StopGrabbing()
	{
		_cursorService.ResetTemporaryCursor();
		_startingLevel = null;
		_startedGrabbing = false;
	}

	private Vector3? DeltaFromStartingTarget(float startingLevel)
	{
		Ray ray = _cameraService.ScreenPointToPreciseRayInWorldSpace(_startingMousePosition);
		if (IntersectsWithLevel(ray, startingLevel))
		{
			Plane plane = new Plane(Vector3.down, startingLevel);
			Ray ray2 = _cameraService.ScreenPointToPreciseRayInWorldSpace(_inputService.MousePosition);
			Vector3? vector = IntersectionWithPlane(ray, plane);
			if (vector.HasValue)
			{
				Vector3 valueOrDefault = vector.GetValueOrDefault();
				vector = IntersectionWithPlane(ray2, plane);
				if (vector.HasValue)
				{
					Vector3 valueOrDefault2 = vector.GetValueOrDefault();
					Vector3 vector2 = valueOrDefault - valueOrDefault2;
					return new Vector3(vector2.x, 0f, vector2.z);
				}
			}
		}
		return null;
	}

	private bool IntersectsWithLevel(Ray ray, float level)
	{
		return GridSpaceRaycasting.HitHorizontalPlane(CoordinateSystem.WorldToGrid(ray), level).HasValue;
	}

	private static Vector3? IntersectionWithPlane(Ray ray, Plane plane)
	{
		if (plane.Raycast(ray, out var enter))
		{
			return ray.GetPoint(enter);
		}
		return null;
	}
}
