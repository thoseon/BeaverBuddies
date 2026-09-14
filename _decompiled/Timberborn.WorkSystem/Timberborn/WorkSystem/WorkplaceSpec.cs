using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WorkSystem;

public record WorkplaceSpec : ComponentSpec
{
	[Serialize]
	public int MaxWorkers { get; init; } = 1;

	[Serialize]
	public int DefaultWorkers { get; init; } = 1;

	[Serialize]
	public string DefaultWorkerType { get; init; } = "Beaver";

	[Serialize]
	public bool DisallowOtherWorkerTypes { get; init; }

	[Serialize]
	public ImmutableArray<WorkerTypeUnlockCost> WorkerTypeUnlockCosts { get; init; }
}
