using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.NeedApplication;

internal record YieldRemoverWorkplaceEffectsSpec : ComponentSpec, INeedEffectsSpec
{
	[Serialize]
	public string YieldGoodId { get; init; }

	[Serialize]
	public int MinimumAttemptsThreshold { get; init; }

	[Serialize]
	public ImmutableArray<NeedApplierEffectSpec> Effects { get; init; }
}
