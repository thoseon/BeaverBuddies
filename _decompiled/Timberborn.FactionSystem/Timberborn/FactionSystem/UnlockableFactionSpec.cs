using Timberborn.BlueprintSystem;

namespace Timberborn.FactionSystem;

public record UnlockableFactionSpec : ComponentSpec
{
	[Serialize]
	public string PrerequisiteFaction { get; init; }

	[Serialize]
	public int AverageWellbeingToUnlock { get; init; }
}
