using Timberborn.BlueprintSystem;

namespace Timberborn.EnterableSystem;

public record EnterableSpec : ComponentSpec
{
	[Serialize]
	public OperatingState OperatingState { get; init; }

	[Serialize]
	public bool LimitedCapacityFinished { get; init; }

	[Serialize]
	public int CapacityFinished { get; init; }

	[Serialize]
	public bool LimitedCapacityUnfinished { get; init; }

	[Serialize]
	public int CapacityUnfinished { get; init; }
}
