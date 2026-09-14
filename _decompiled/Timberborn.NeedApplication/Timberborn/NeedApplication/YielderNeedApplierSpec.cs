using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.NeedApplication;

internal record YielderNeedApplierSpec : ComponentSpec, INeedEffectsSpec
{
	[Serialize]
	public ImmutableArray<NeedApplierEffectSpec> Effects { get; init; }
}
