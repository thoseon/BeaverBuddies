using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.NeedApplication;

internal record ProbabilityGroupsSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<ProbabilityGroupSpec> Groups { get; init; }
}
