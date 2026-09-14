using Timberborn.BlueprintSystem;

namespace Timberborn.Yielding;

public record YieldRemovingBuildingSpec : ComponentSpec
{
	[Serialize]
	public string ResourceGroup { get; init; }
}
