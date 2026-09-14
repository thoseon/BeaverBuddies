using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

internal record TransformSlotInitializerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<TransformSlotSpec> Slots { get; init; }
}
