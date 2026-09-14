using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.TimbermeshAnimations;

internal record AnimatorState
{
	[Serialize]
	public string StateName { get; init; }

	[Serialize]
	public string AnimationName { get; init; }

	[Serialize]
	public float Speed { get; init; } = 1f;

	[Serialize]
	public string SpeedModifier { get; init; }

	[Serialize]
	public bool Looped { get; init; } = true;

	[Serialize]
	public ImmutableArray<AnimatorStateCondition> Conditions { get; init; }
}
