using Timberborn.BlueprintSystem;

namespace Timberborn.TickSystem;

internal record TickTimeSpec : ComponentSpec
{
	[Serialize]
	public float TickIntervalInSeconds { get; init; }
}
