using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

internal record PrioritySlotRetrieverSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> PrioritySlotNames { get; init; }
}
