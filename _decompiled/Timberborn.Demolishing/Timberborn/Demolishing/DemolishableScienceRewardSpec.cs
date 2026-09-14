using Timberborn.BlueprintSystem;

namespace Timberborn.Demolishing;

public record DemolishableScienceRewardSpec : ComponentSpec
{
	[Serialize]
	public int SciencePoints { get; init; }
}
