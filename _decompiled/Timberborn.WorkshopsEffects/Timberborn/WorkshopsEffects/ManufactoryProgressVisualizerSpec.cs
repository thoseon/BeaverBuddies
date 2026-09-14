using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WorkshopsEffects;

internal record ManufactoryProgressVisualizerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<ProgressStepSpec> ProgressSteps { get; init; }
}
