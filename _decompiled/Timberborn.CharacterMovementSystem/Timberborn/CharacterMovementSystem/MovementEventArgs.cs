using Timberborn.Navigation;

namespace Timberborn.CharacterMovementSystem;

public readonly struct MovementEventArgs
{
	public PathCorner From { get; }

	public PathCorner To { get; }

	public PathCorner? Next { get; }

	public MovementEventArgs(PathCorner from, PathCorner to, PathCorner? next)
	{
		From = from;
		To = to;
		Next = next;
	}
}
