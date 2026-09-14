using Timberborn.BlueprintSystem;

namespace Timberborn.DeteriorationSystem;

internal record DeteriorableSpec : ComponentSpec
{
	[Serialize]
	public int DeteriorationInDays { get; init; }
}
