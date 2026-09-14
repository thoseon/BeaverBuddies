using Timberborn.BlueprintSystem;

namespace Timberborn.RangedEffectSystem;

internal record RangedEffectBuildingSpec : ComponentSpec
{
	[Serialize]
	public int EffectRadius { get; init; }
}
