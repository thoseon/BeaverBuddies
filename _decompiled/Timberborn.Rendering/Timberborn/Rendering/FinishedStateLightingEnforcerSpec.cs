using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Rendering;

internal record FinishedStateLightingEnforcerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> ChildrenNames { get; init; }
}
