using System;
using Timberborn.CameraSystem;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;

namespace Timberborn.TutorialSteps;

internal class CameraMovementService : ILoadableSingleton
{
	private readonly CameraService _cameraService;

	private readonly EventBus _eventBus;

	private CameraState _lastCameraState;

	public float UpMovement { get; private set; }

	public float DownMovement { get; private set; }

	public float LeftMovement { get; private set; }

	public float RightMovement { get; private set; }

	public float LeftRotation { get; private set; }

	public float RightRotation { get; private set; }

	public float ZoomIn { get; private set; }

	public float ZoomOut { get; private set; }

	public CameraMovementService(CameraService cameraService, EventBus eventBus)
	{
		_cameraService = cameraService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_lastCameraState = _cameraService.GetCurrentState();
		_cameraService.BeforeCameraUpdate += OnBeforeCameraUpdate;
	}

	[OnEvent]
	public void OnTutorialStageStarted(TutorialStageStartedEvent tutorialStageStartedEvent)
	{
		UpMovement = 0f;
		DownMovement = 0f;
		LeftMovement = 0f;
		RightMovement = 0f;
		LeftRotation = 0f;
		RightRotation = 0f;
		ZoomIn = 0f;
		ZoomOut = 0f;
		_lastCameraState = _cameraService.GetCurrentState();
	}

	private void OnBeforeCameraUpdate(object sender, EventArgs e)
	{
		CameraState currentState = _cameraService.GetCurrentState();
		if (currentState != _lastCameraState)
		{
			Update(currentState);
		}
	}

	private void Update(CameraState currentCameraState)
	{
		Vector3 vector = currentCameraState.Target - _lastCameraState.Target;
		float num = currentCameraState.ZoomLevel - _lastCameraState.ZoomLevel;
		float num2 = currentCameraState.HorizontalAngle - _lastCameraState.HorizontalAngle;
		Vector3 vector2 = Quaternion.Euler(0f, 0f - currentCameraState.HorizontalAngle, 0f) * vector;
		UpMovement += ((vector2.z > 0f) ? vector2.z : 0f);
		DownMovement += ((vector2.z < 0f) ? (0f - vector2.z) : 0f);
		LeftMovement += ((vector2.x < 0f) ? (0f - vector2.x) : 0f);
		RightMovement += ((vector2.x > 0f) ? vector2.x : 0f);
		RightRotation += ((num2 < 0f) ? (0f - num2) : 0f);
		LeftRotation += ((num2 > 0f) ? num2 : 0f);
		ZoomIn += ((num < 0f) ? (0f - num) : 0f);
		ZoomOut += ((num > 0f) ? num : 0f);
		_lastCameraState = currentCameraState;
	}
}
