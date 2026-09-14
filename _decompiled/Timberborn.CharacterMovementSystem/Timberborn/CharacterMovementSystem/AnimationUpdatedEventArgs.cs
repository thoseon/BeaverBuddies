namespace Timberborn.CharacterMovementSystem;

public readonly struct AnimationUpdatedEventArgs
{
	public float AnimationSpeed { get; }

	public AnimationUpdatedEventArgs(float animationSpeed)
	{
		AnimationSpeed = animationSpeed;
	}
}
