using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.DwellingSystem;

internal record DwellingSpec : ComponentSpec
{
	[Serialize]
	public int MaxBeavers { get; init; }

	[Serialize]
	public ImmutableArray<ContinuousEffectSpec> SleepEffects { get; init; }
}
