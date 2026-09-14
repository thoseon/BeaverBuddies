using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Wandering;

internal record VariedIdleAnimationSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> Variants { get; init; }
}
