using Timberborn.BlueprintSystem;

namespace Timberborn.Characters;

internal record GameSpeedThrottlerSpec : ComponentSpec
{
	[Serialize]
	public int MinPopulation { get; init; }

	[Serialize]
	public int MaxPopulation { get; init; }

	[Serialize]
	public float MinGameSpeedScale { get; init; }

	[Serialize]
	public float MaxGameSpeedScale { get; init; }
}
