using Timberborn.BlueprintSystem;

namespace Timberborn.WaterSourceSystem;

public record WaterDepthStrengthModifierSpec : ComponentSpec
{
	[Serialize]
	public float DepthLimit { get; init; } = 0.8f;
}
