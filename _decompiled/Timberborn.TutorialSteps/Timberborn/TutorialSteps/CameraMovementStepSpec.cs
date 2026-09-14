using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;

namespace Timberborn.TutorialSteps;

internal record CameraMovementStepSpec : ComponentSpec
{
	[Serialize]
	public Direction2D Direction { get; init; }

	[Serialize]
	public float Threshold { get; init; }
}
