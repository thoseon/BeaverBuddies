using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

public record PatrollingSlotSpec
{
	[Serialize]
	public float BaseMovementSpeed { get; init; }

	[Serialize]
	public float MaxRandomDeviationOfMovementSpeed { get; init; }

	[Serialize]
	public string SlotKeyword { get; init; }

	[Serialize]
	public string Animation { get; init; }

	[Serialize]
	public bool WaterSlot { get; init; }
}
