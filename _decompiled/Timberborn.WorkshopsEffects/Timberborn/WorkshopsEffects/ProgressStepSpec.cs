using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WorkshopsEffects;

internal record ProgressStepSpec
{
	[Serialize]
	public float Threshold { get; init; }

	[Serialize]
	public ImmutableArray<string> ModelNames { get; init; }
}
