using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;

namespace Timberborn.BuildingsNavigation;

internal record DistanceToColorConverterSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<GradientPointSpec> DistanceGradient { get; init; }
}
