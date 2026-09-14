using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Rendering;

internal record MapBottomGroundCutoffSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> Targets { get; init; }
}
