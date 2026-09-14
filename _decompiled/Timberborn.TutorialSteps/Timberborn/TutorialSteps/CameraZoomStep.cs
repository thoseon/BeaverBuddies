using System;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class CameraZoomStep : ITutorialStep
{
	private readonly CameraMovementService _cameraMovementService;

	private readonly ZoomDirection _direction;

	private readonly float _threshold;

	private readonly string _description;

	public CameraZoomStep(CameraMovementService cameraMovementService, ZoomDirection direction, float threshold, string description)
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
			ZoomDirection.In => _cameraMovementService.ZoomIn > _threshold, 
			ZoomDirection.Out => _cameraMovementService.ZoomOut > _threshold, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
