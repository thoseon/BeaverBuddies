namespace Timberborn.WalkingSystem;

public readonly struct StartedNewPathEventArgs
{
	public float Distance { get; }

	public StartedNewPathEventArgs(float distance)
	{
		Distance = distance;
	}
}
