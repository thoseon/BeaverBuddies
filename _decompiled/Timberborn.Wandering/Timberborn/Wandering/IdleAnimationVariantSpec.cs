using Timberborn.BlueprintSystem;

namespace Timberborn.Wandering;

internal record IdleAnimationVariantSpec
{
	[Serialize]
	public float Probability { get; init; }

	[Serialize]
	public string Variant { get; init; }
}
