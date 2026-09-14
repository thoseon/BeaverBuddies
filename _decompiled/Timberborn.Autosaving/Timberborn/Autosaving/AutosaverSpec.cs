using Timberborn.BlueprintSystem;

namespace Timberborn.Autosaving;

internal record AutosaverSpec : ComponentSpec
{
	[Serialize]
	public int AutosavesPerSettlement { get; init; }

	[Serialize]
	public float FrequencyInMinutes { get; init; }
}
