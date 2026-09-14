using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

internal record UnfinishedStateSlotDisablerSpec : ComponentSpec
{
	[Serialize]
	public string SlotKeyword { get; init; }
}
