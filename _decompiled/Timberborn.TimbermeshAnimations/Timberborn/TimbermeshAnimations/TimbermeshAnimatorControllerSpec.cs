using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;

namespace Timberborn.TimbermeshAnimations;

internal record TimbermeshAnimatorControllerSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> BoolParameters { get; init; }

	[Serialize]
	public ImmutableArray<string> FloatParameters { get; init; }

	[Serialize]
	public ImmutableArray<AnimatorState> AnimatorStates { get; init; }

	public IEnumerable<string> UsedBoolParameters => (from c in AnimatorStates.SelectMany((AnimatorState s) => s.Conditions)
		select c.ParameterName).Distinct();

	public IEnumerable<string> UsedFloatParameters => AnimatorStates.Select((AnimatorState s) => s.SpeedModifier).Distinct();

	public IEnumerable<string> AnimationNames => AnimatorStates.Select((AnimatorState s) => s.AnimationName).Distinct();
}
