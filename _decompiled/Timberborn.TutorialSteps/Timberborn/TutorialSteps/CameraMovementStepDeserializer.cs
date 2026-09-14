using System;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class CameraMovementStepDeserializer : IStepDeserializer
{
	private readonly CameraMovementService _cameraMovementService;

	private readonly ILoc _loc;

	private readonly InputService _inputService;

	private readonly InputSettings _inputSettings;

	public CameraMovementStepDeserializer(CameraMovementService cameraMovementService, ILoc loc, InputService inputService, InputSettings inputSettings)
	{
		_cameraMovementService = cameraMovementService;
		_loc = loc;
		_inputService = inputService;
		_inputSettings = inputSettings;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is CameraMovementStepSpec cameraMovementStepSpec)
		{
			tutorialStep = Create(cameraMovementStepSpec.Direction, cameraMovementStepSpec.Threshold);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(Direction2D direction, float threshold)
	{
		return TutorialStep.Create(new CameraMovementStep(_cameraMovementService, direction, threshold, _loc.T(GetLocKey(direction))), fixedKeyBinding: GetFixedKeyBindingKey(direction, _inputService.MouseMoveCameraKey), keyBinding: GetKeyBindingKey(direction));
	}

	private static string GetLocKey(Direction2D direction)
	{
		return direction switch
		{
			Direction2D.Down => "Tutorial.Basics.MoveCameraDown", 
			Direction2D.Left => "Tutorial.Basics.MoveCameraLeft", 
			Direction2D.Up => "Tutorial.Basics.MoveCameraUp", 
			Direction2D.Right => "Tutorial.Basics.MoveCameraRight", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private static string GetKeyBindingKey(Direction2D direction)
	{
		return direction switch
		{
			Direction2D.Down => "MoveCameraDown", 
			Direction2D.Left => "MoveCameraLeft", 
			Direction2D.Up => "MoveCameraUp", 
			Direction2D.Right => "MoveCameraRight", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private string GetFixedKeyBindingKey(Direction2D direction, string button)
	{
		return button + "|" + GetFixedKeyBindingDirectionKey(direction);
	}

	private string GetFixedKeyBindingDirectionKey(Direction2D direction)
	{
		return direction switch
		{
			Direction2D.Down => _inputSettings.DragCamera ? "Down" : "Up", 
			Direction2D.Left => _inputSettings.DragCamera ? "Left" : "Right", 
			Direction2D.Up => _inputSettings.DragCamera ? "Up" : "Down", 
			Direction2D.Right => _inputSettings.DragCamera ? "Right" : "Left", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
