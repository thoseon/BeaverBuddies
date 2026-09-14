using System;
using Timberborn.BlueprintSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class CameraRotationStepDeserializer : IStepDeserializer
{
	private readonly CameraMovementService _cameraMovementService;

	private readonly ILoc _loc;

	private readonly InputService _inputService;

	public CameraRotationStepDeserializer(CameraMovementService cameraMovementService, ILoc loc, InputService inputService)
	{
		_cameraMovementService = cameraMovementService;
		_loc = loc;
		_inputService = inputService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is CameraRotationStepSpec cameraRotationStepSpec)
		{
			tutorialStep = Create(cameraRotationStepSpec.Direction, cameraRotationStepSpec.Angle);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(RotationDirection direction, float angle)
	{
		return TutorialStep.Create(new CameraRotationStep(_cameraMovementService, direction, angle, GetDescription(direction)), fixedKeyBinding: GetFixedKeyBindingKey(direction, _inputService.MouseRotateCameraKey), keyBinding: GetKeyBindingKey(direction));
	}

	private string GetDescription(RotationDirection direction)
	{
		return _loc.T(GetLocKey(direction));
	}

	private static string GetLocKey(RotationDirection direction)
	{
		return direction switch
		{
			RotationDirection.Left => "Tutorial.Basics.RotateCameraLeft", 
			RotationDirection.Right => "Tutorial.Basics.RotateCameraRight", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private static string GetKeyBindingKey(RotationDirection direction)
	{
		return direction switch
		{
			RotationDirection.Left => "RotateCameraLeft", 
			RotationDirection.Right => "RotateCameraRight", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private static string GetFixedKeyBindingKey(RotationDirection direction, string button)
	{
		return button + "|" + GetFixedKeyBindingDirectionKey(direction);
	}

	private static string GetFixedKeyBindingDirectionKey(RotationDirection direction)
	{
		return direction switch
		{
			RotationDirection.Left => "Right", 
			RotationDirection.Right => "Left", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
