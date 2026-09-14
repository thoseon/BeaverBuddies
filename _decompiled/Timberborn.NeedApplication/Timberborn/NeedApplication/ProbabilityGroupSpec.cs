using Timberborn.BlueprintSystem;

namespace Timberborn.NeedApplication;

internal record ProbabilityGroupSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public float Low { get; init; }

	[Serialize]
	public float Medium { get; init; }

	[Serialize]
	public float High { get; init; }
}
