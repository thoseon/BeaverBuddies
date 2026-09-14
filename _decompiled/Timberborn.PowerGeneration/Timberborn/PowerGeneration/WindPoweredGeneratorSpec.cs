using Timberborn.BlueprintSystem;

namespace Timberborn.PowerGeneration;

internal record WindPoweredGeneratorSpec : ComponentSpec
{
	[Serialize]
	public float MinRequiredWindStrength { get; init; }
}
