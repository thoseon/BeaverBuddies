using Timberborn.BlueprintSystem;

namespace Timberborn.Pollination;

internal record HiveSpec : ComponentSpec
{
	[Serialize]
	public int PollinationRadius { get; init; }

	[Serialize]
	public float HoursBetweenPollinations { get; init; }

	[Serialize]
	public float GrowthTimeReduction { get; init; }

	[Serialize]
	public int PlantsPerPollination { get; init; }
}
