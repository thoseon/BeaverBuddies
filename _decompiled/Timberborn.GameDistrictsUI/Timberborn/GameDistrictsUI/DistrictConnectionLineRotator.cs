using System;
using Timberborn.CameraSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal class DistrictConnectionLineRotator : IUpdatableSingleton
{
	private static readonly float TiltStartingAngle = 35f;

	private static readonly float MaxTilt = 45f;

	private readonly CameraService _cameraService;

	private Transform _transformToRotate;

	private Quaternion _startingRotation;

	private Vector3 _upVector;

	private Vector3 _rightVector;

	private Vector3 _forwardVector;

	private Vector3 _start;

	private bool _simpleRotation;

	private bool _enabled;

	public DistrictConnectionLineRotator(CameraService cameraService)
	{
		_cameraService = cameraService;
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			if (_simpleRotation)
			{
				SimpleRotation();
			}
			else
			{
				Rotation();
			}
		}
	}

	public void StartRotatingSimple(Vector3 start, Vector3 end, Transform transformToRotate)
	{
		_transformToRotate = transformToRotate;
		SetLineDirection(start, end);
		_start = start;
		_simpleRotation = true;
		_enabled = true;
	}

	public void StartRotating(Vector3 start, Vector3 end, Transform transformToRotate)
	{
		_transformToRotate = transformToRotate;
		SetLineDirection(start, end);
		_rightVector = _transformToRotate.right;
		_upVector = _transformToRotate.up;
		_forwardVector = _transformToRotate.forward;
		_simpleRotation = false;
		_enabled = true;
	}

	public void StopRotating()
	{
		_transformToRotate = null;
		_enabled = false;
	}

	private void SetLineDirection(Vector3 start, Vector3 end)
	{
		Vector3 normalized = (end - start).normalized;
		Vector3 rhs = Vector3.Cross(normalized, Vector3.up);
		Vector3 forward = Vector3.Cross(normalized, rhs);
		_startingRotation = Quaternion.LookRotation(forward, Vector3.Cross(Vector3.up, rhs));
		_transformToRotate.rotation = _startingRotation;
	}

	private void SimpleRotation()
	{
		Vector3 forward = _cameraService.Transform.position - _start;
		forward.y = 0f;
		_transformToRotate.rotation = Quaternion.LookRotation(forward);
	}

	private void Rotation()
	{
		Vector3 normalized = _cameraService.Transform.forward.normalized;
		float t = (Vector3.SignedAngle(_forwardVector, normalized, Vector3.Cross(_forwardVector, normalized)) + TiltStartingAngle) / (180f - TiltStartingAngle);
		float num = Mathf.Sign(Vector3.Dot(-_rightVector, normalized));
		float xZTiltRatio = GetXZTiltRatio(normalized);
		float num2 = Mathf.Lerp(0f, MaxTilt, t) * num * xZTiltRatio;
		_transformToRotate.rotation = Quaternion.AngleAxis(0f - num2, _upVector) * _startingRotation;
	}

	private float GetXZTiltRatio(Vector3 cameraForward)
	{
		Vector3 vector = cameraForward;
		vector.y = 0f;
		Vector3 upVector = _upVector;
		upVector.y = 0f;
		float num = Vector3.Dot(vector.normalized, upVector.normalized);
		return 1f - (float)Math.Pow(num, 4.0);
	}
}
