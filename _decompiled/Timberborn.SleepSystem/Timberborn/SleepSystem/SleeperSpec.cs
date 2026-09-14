using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.SleepSystem;

internal record SleeperSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<ContinuousEffectSpec> SleepOutsideEffects { get; init; }

	[Serialize]
	public float MaxOffsetInHours { get; init; }
}
