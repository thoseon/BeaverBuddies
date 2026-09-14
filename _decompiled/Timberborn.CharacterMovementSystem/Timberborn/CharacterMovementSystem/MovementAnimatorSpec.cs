using Timberborn.BlueprintSystem;

namespace Timberborn.CharacterMovementSystem;

internal record MovementAnimatorSpec : ComponentSpec
{
	[Serialize]
	public float AnimationSpeedScale { get; init; }
}
