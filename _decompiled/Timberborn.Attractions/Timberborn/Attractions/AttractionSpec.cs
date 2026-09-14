using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.Attractions;

internal record AttractionSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<ContinuousEffectSpec> Effects { get; init; }
}
