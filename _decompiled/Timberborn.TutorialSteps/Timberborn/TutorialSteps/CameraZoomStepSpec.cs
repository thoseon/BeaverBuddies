using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record CameraZoomStepSpec : ComponentSpec
{
	[Serialize]
	public ZoomDirection Direction { get; init; }

	[Serialize]
	public float Threshold { get; init; }
}
