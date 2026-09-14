using System;
using Timberborn.CameraSystem;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class CameraTargeter : ILoadableSingleton, IInputProcessor
{
	private readonly InputService _inputService;

	private readonly EventBus _eventBus;

	private readonly CameraService _cameraService;

	private Vector3 _lastPositionCenteredOn;

	private float _distanceToTarget;

	private float _previousZoomLevel;

	private float _zoomDelta;

	public SelectableObject FollowedTarget { get; private set; }

	private bool OtherControllerModifiedCameraTarget => !_cameraService.Target.Equals(_lastPositionCenteredOn);

	public CameraTargeter(InputService inputService, EventBus eventBus, CameraService cameraService)
	{
		_inputService = inputService;
		_eventBus = eventBus;
		_cameraService = cameraService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_cameraService.BeforeCameraUpdate += OnBeforeCameraUpdate;
		_inputService.AddInputProcessor(this);
	}

	[OnEvent]
	public void OnSelectableObjectUnselectedEvent(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		if ((bool)FollowedTarget && FollowedTarget == selectableObjectUnselectedEvent.SelectableObject)
		{
			StopFollowing();
		}
	}

	public void Follow(SelectableObject targetToFollow)
	{
		FollowedTarget = targetToFollow;
		_previousZoomLevel = _cameraService.ZoomLevel;
		CenterCameraOnFollowedTarget(updateZoom: false);
		Vector3 a = _cameraService.Target + _cameraService.OffsetFromTarget;
		_distanceToTarget = Vector3.Distance(a, FollowedTarget.CameraTargetPosition);
	}

	public void StopFollowing()
	{
		FollowedTarget = null;
		_zoomDelta = 0f;
		_distanceToTarget = 0f;
		_previousZoomLevel = 0f;
	}

	public void CenterCameraOn(SelectableObject target)
	{
		CenterCameraOn(target.CameraTargetPosition, updateZoom: false);
	}

	public bool ProcessInput()
	{
		if (_inputService.Cancel)
		{
			StopFollowing();
		}
		return false;
	}

	private void OnBeforeCameraUpdate(object sender, EventArgs e)
	{
		if ((bool)FollowedTarget)
		{
			if (OtherControllerModifiedCameraTarget)
			{
				StopFollowing();
			}
			else
			{
				CenterCameraOnFollowedTarget(updateZoom: true);
			}
		}
	}

	private void CenterCameraOnFollowedTarget(bool updateZoom)
	{
		CenterCameraOn(FollowedTarget.CameraTargetPosition, updateZoom);
	}

	private void CenterCameraOn(Vector3 targetPosition, bool updateZoom)
	{
		Vector3 vector = targetPosition + _cameraService.OffsetFromTarget;
		Ray ray = new Ray(vector, targetPosition - vector);
		if (new Plane(Vector3.down, 0f).Raycast(ray, out var enter))
		{
			Vector3 point = ray.GetPoint(enter);
			_cameraService.MoveTargetTo(point);
			_lastPositionCenteredOn = _cameraService.Target;
			if (updateZoom)
			{
				UpdateZoom(targetPosition, point);
			}
		}
	}

	private void UpdateZoom(Vector3 targetPosition, Vector3 hitPoint)
	{
		_zoomDelta += _cameraService.ZoomLevel - _previousZoomLevel;
		float distanceFromTarget = Vector3.Distance(hitPoint, targetPosition) + _distanceToTarget;
		_cameraService.SetZoomLevel(distanceFromTarget, _zoomDelta);
		_previousZoomLevel = _cameraService.ZoomLevel;
	}
}
