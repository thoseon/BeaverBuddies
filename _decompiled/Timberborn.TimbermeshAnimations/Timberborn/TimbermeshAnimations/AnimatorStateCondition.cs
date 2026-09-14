using Timberborn.BlueprintSystem;

namespace Timberborn.TimbermeshAnimations;

internal record AnimatorStateCondition
{
	[Serialize]
	public string ParameterName { get; init; }

	[Serialize]
	public bool MustBeTrue { get; init; }
}
