using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.RangedEffectSystem;

internal record ContinuousEffectBuildingSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<ContinuousEffectSpec> Effects { get; init; }
}
