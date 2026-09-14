using Timberborn.BlueprintSystem;

namespace Timberborn.Growing;

public record GrowableSpec : ComponentSpec
{
	[Serialize]
	public float GrowthTimeInDays { get; init; }
}
