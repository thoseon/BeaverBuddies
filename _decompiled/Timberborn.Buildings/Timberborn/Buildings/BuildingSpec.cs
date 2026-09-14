using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Goods;

namespace Timberborn.Buildings;

public record BuildingSpec : ComponentSpec
{
	[Serialize]
	public string SelectionSoundName { get; init; } = "Default";

	[Serialize]
	public string LoopingSoundName { get; init; }

	[Serialize]
	public ImmutableArray<GoodAmountSpec> BuildingCost { get; init; }

	[Serialize]
	public int ScienceCost { get; init; }

	[Serialize]
	public bool PlaceFinished { get; init; }

	[Serialize]
	public bool FinishableWithBeaversOnSite { get; init; }

	[Serialize]
	public bool DrawRangeBoundsOnIt { get; init; }
}
