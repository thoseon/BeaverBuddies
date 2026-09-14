using System;
using Timberborn.Coordinates;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class CameraMovementStep : ITutorialStep
{
	private readonly CameraMovementService _cameraMovementService;

	private readonly Direction2D _direction;

	private readonly float _threshold;

	private readonly string _description;

	public CameraMovementStep(CameraMovementService cameraMovementService, Direction2D direction, float threshold, string description)
	{
		_cameraMovementService = cameraMovementService;
		_direction = direction;
		_threshold = threshold;
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
			Direction2D.Down => _cameraMovementService.DownMovement > _threshold, 
			Direction2D.Left => _cameraMovementService.LeftMovement > _threshold, 
			Direction2D.Up => _cameraMovementService.UpMovement > _threshold, 
			Direction2D.Right => _cameraMovementService.RightMovement > _threshold, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
