using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Yielding;

internal record RemoveYieldStrategySpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public ImmutableArray<string> CompatibleResourceGroups { get; init; }

	[Serialize]
	public string Animation { get; init; }
}
