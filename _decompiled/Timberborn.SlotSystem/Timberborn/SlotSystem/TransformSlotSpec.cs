using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

public record TransformSlotSpec
{
	[Serialize]
	public string SlotKeyword { get; init; }

	[Serialize]
	public string Animation { get; init; }

	[Serialize]
	public bool Inanimate { get; init; }

	[Serialize]
	public bool RandomizeYRotation { get; init; }

	[Serialize]
	public bool WaterSlot { get; init; }
}
