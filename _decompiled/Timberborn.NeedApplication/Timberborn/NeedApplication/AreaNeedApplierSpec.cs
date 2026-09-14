using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.NeedApplication;

internal record AreaNeedApplierSpec : ComponentSpec, INeedEffectsSpec
{
	[Serialize]
	public int ApplicationRadius { get; init; }

	[Serialize]
	public ImmutableArray<NeedApplierEffectSpec> Effects { get; init; }
}
