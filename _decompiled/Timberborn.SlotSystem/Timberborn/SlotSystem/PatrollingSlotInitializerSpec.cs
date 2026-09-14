using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

internal record PatrollingSlotInitializerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<PatrollingSlotSpec> PatrollingSlots { get; init; }
}
