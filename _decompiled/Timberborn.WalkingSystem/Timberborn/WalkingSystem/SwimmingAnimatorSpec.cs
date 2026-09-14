using Timberborn.BlueprintSystem;

namespace Timberborn.WalkingSystem;

internal record SwimmingAnimatorSpec : ComponentSpec
{
	[Serialize]
	public float LowerSwimmingDepthThreshold { get; init; }

	[Serialize]
	public float UpperSwimmingDepthThreshold { get; init; }
}
