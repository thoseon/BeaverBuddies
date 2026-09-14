using Timberborn.BlueprintSystem;

namespace Timberborn.ConstructionGuidelines;

internal record ConstructionGuidelinesSpec : ComponentSpec
{
	[Serialize]
	public int Radius { get; init; }
}
