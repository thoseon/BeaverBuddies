using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.NeedApplication;

internal record WorkshopRandomNeedApplierSpec : ComponentSpec, INeedEffectsSpec
{
	[Serialize]
	public ImmutableArray<NeedApplierEffectSpec> Effects { get; init; }
}
