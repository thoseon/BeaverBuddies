namespace Timberborn.RangedEffectSystem;

internal readonly struct ActiveChangedEventArgs
{
	public bool State { get; }

	public ActiveChangedEventArgs(bool state)
	{
		State = state;
	}
}
