using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.NeedApplication;

internal record DemolishableEffectsSpec : ComponentSpec, INeedEffectsSpec
{
	[Serialize]
	public ImmutableArray<NeedApplierEffectSpec> Effects { get; init; }
}
