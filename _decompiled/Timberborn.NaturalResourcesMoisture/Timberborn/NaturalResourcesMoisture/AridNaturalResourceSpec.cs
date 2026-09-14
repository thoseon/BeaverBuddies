using Timberborn.BlueprintSystem;

namespace Timberborn.NaturalResourcesMoisture;

public record AridNaturalResourceSpec : ComponentSpec
{
	[Serialize]
	public float DaysToDieWet { get; init; }
}
