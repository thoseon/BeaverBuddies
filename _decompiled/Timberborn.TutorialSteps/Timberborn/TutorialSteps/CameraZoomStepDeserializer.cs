using System;
using Timberborn.BlueprintSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class CameraZoomStepDeserializer : IStepDeserializer
{
	private readonly CameraMovementService _cameraMovementService;

	private readonly ILoc _loc;

	private readonly InputSettings _inputSettings;

	public CameraZoomStepDeserializer(CameraMovementService cameraMovementService, ILoc loc, InputSettings inputSettings)
	{
		_cameraMovementService = cameraMovementService;
		_loc = loc;
		_inputSettings = inputSettings;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is CameraZoomStepSpec cameraZoomStepSpec)
		{
			tutorialStep = Create(cameraZoomStepSpec.Direction, cameraZoomStepSpec.Threshold);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(ZoomDirection direction, float threshold)
	{
		return TutorialStep.Create(new CameraZoomStep(_cameraMovementService, direction, threshold, GetDescription(direction)), GetKeyBindingKey(direction), GetFixedKeyBindingKey(direction, "MouseZoom"));
	}

	private string GetDescription(ZoomDirection direction)
	{
		return _loc.T(GetLocKey(direction));
	}

	private static string GetLocKey(ZoomDirection direction)
	{
		return direction switch
		{
			ZoomDirection.In => "Tutorial.Basics.ZoomCameraIn", 
			ZoomDirection.Out => "Tutorial.Basics.ZoomCameraOut", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private static string GetKeyBindingKey(ZoomDirection direction)
	{
		return direction switch
		{
			ZoomDirection.In => "ZoomIn", 
			ZoomDirection.Out => "ZoomOut", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private string GetFixedKeyBindingKey(ZoomDirection direction, string button)
	{
		return button + "|" + GetFixedKeyBindingDirectionKey(direction);
	}

	private string GetFixedKeyBindingDirectionKey(ZoomDirection direction)
	{
		bool flag = ApplicationPlatform.IsMacOS();
		bool invertZoom = _inputSettings.InvertZoom;
		bool flag2 = (invertZoom && !flag) || (!invertZoom & flag);
		return direction switch
		{
			ZoomDirection.In => flag2 ? "ScrollDown" : "ScrollUp", 
			ZoomDirection.Out => flag2 ? "ScrollUp" : "ScrollDown", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
