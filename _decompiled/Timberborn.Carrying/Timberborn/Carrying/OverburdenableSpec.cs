using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.BonusSystem;

namespace Timberborn.Carrying;

internal record OverburdenableSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<BonusSpec> OverburdenedBonuses { get; init; }
}
