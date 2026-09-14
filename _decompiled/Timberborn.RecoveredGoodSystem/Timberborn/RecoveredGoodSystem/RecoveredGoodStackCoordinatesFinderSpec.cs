using Timberborn.BlueprintSystem;

namespace Timberborn.RecoveredGoodSystem;

internal record RecoveredGoodStackCoordinatesFinderSpec : ComponentSpec
{
	[Serialize]
	public int NeighboursRange { get; init; }

	[Serialize]
	public int MaxUpperSearch { get; init; }
}
