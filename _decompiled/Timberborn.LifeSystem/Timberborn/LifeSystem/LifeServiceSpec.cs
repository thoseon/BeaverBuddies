using Timberborn.BlueprintSystem;

namespace Timberborn.LifeSystem;

internal record LifeServiceSpec : ComponentSpec
{
	[Serialize]
	public int AverageLifespan { get; init; }

	[Serialize]
	public int DaysOfChildhood { get; init; }
}
