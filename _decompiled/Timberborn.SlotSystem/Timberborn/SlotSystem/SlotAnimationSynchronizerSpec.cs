using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

internal record SlotAnimationSynchronizerSpec : ComponentSpec
{
	[Serialize]
	public float MaxTimeOffset { get; init; }
}
