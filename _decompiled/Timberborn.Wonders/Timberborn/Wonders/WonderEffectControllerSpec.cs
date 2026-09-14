using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.Wonders;

internal record WonderEffectControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<ContinuousEffectSpec> Effects { get; init; }
}
