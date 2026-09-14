using Timberborn.BlueprintSystem;

namespace Timberborn.WorkerOutfitSystem;

internal record WorkplaceWorkerOutfitSpec : ComponentSpec
{
	[Serialize]
	public string WorkerOutfit { get; init; }
}
