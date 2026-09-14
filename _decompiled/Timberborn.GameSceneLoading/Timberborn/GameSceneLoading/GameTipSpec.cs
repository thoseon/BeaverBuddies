using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.GameSceneLoading;

internal record GameTipSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> Tips { get; init; }
}
