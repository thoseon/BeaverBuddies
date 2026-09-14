using System;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CameraSystem;

public class CameraHorizontalShifter : IUpdatableSingleton
{
	private readonly CameraService _cameraService;

	private float? _targetOffset;

	public float CurrentOffset { get; private set; }

	public CameraHorizontalShifter(CameraService cameraService)
	{
		_cameraService = cameraService;
	}

	public void UpdateSingleton()
	{
		if (!_targetOffset.HasValue)
		{
			return;
		}
		float value = _targetOffset.Value;
		float num = Math.Min(Time.unscaledDeltaTime, 0.05f);
		CurrentOffset = Mathf.Lerp(CurrentOffset, value, 6f * num);
		Matrix4x4 projectionMatrix = _cameraService.ProjectionMatrix;
		projectionMatrix[0, 2] = CurrentOffset;
		_cameraService.SetProjectionMatrix(projectionMatrix);
		if (Math.Abs(CurrentOffset - value) < 0.0001f)
		{
			_targetOffset = null;
			if (value == 0f)
			{
				_cameraService.ResetProjectionMatrix();
			}
		}
	}

	public void EnableHorizontalCameraShift(float offset)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity[0, 2] = offset;
		identity[1, 2] = 0f;
		_targetOffset = (_cameraService.ProjectionMatrix * identity)[0, 2];
	}

	public void DisableCameraShift()
	{
		_targetOffset = 0f;
	}
}
