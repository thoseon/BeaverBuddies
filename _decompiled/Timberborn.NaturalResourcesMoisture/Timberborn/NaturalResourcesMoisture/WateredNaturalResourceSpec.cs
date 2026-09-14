using Timberborn.BlueprintSystem;

namespace Timberborn.NaturalResourcesMoisture;

public record WateredNaturalResourceSpec : ComponentSpec
{
	[Serialize]
	public float DaysToDieDry { get; init; }
}
