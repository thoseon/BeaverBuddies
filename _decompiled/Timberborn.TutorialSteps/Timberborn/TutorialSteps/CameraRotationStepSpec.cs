using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record CameraRotationStepSpec : ComponentSpec
{
	[Serialize]
	public RotationDirection Direction { get; init; }

	[Serialize]
	public float Angle { get; init; }
}
