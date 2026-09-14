using Timberborn.BlueprintSystem;

namespace Timberborn.NaturalResourcesMoisture;

public record FloodableNaturalResourceSpec : ComponentSpec
{
	[Serialize]
	public int MinWaterHeight { get; init; }

	[Serialize]
	public int MaxWaterHeight { get; init; }

	[Serialize]
	public float DaysToDie { get; init; }
}
