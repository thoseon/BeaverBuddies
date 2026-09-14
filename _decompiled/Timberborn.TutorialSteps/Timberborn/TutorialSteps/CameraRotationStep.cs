using System;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class CameraRotationStep : ITutorialStep
{
	private readonly CameraMovementService _cameraMovementService;

	private readonly RotationDirection _direction;

	private readonly float _angle;

	private readonly string _description;

	public CameraRotationStep(CameraMovementService cameraMovementService, RotationDirection direction, float angle, string description)
	{
		_cameraMovementService = cameraMovementService;
		_direction = direction;
		_angle = angle;
		_description = description;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		return _direction switch
		{
			RotationDirection.Left => _cameraMovementService.LeftRotation > _angle, 
			RotationDirection.Right => _cameraMovementService.RightRotation > _angle, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
