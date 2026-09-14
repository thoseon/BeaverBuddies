using Timberborn.BlueprintSystem;

namespace Timberborn.MechanicalSystemUI;

internal record MechanicalNodeAnimatorSpec : ComponentSpec
{
	[Serialize]
	public float MinSpeedMultiplier { get; init; }
}
