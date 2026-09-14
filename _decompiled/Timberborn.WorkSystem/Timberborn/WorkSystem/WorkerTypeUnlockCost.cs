using Timberborn.BlueprintSystem;

namespace Timberborn.WorkSystem;

public record WorkerTypeUnlockCost
{
	[Serialize]
	public string WorkerType { get; init; }

	[Serialize]
	public int ScienceCost { get; init; }
}
