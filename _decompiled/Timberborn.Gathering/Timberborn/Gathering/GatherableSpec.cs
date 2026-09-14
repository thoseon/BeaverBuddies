using Timberborn.BlueprintSystem;
using Timberborn.Yielding;

namespace Timberborn.Gathering;

public record GatherableSpec : ComponentSpec, IYielderDecorable
{
	[Serialize]
	public float YieldGrowthTimeInDays { get; init; }

	[Serialize]
	public YielderSpec Yielder { get; init; }
}
