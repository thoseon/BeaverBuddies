using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Goods;

namespace Timberborn.Reproduction;

internal record BreedingPodSpec : ComponentSpec
{
	[Serialize]
	public string EmbryoName { get; init; }

	[Serialize]
	public float CycleLengthInDays { get; init; }

	[Serialize]
	public int CyclesUntilFullyGrown { get; init; }

	[Serialize]
	public int CyclesCapacity { get; init; }

	[Serialize]
	public ImmutableArray<GoodAmountSpec> NutrientsPerCycle { get; init; }

	[Serialize]
	public bool SpawnAdults { get; init; }
}
