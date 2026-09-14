namespace Timberborn.ScienceSystem;

public class NotEnoughScienceStateChangedEventArgs
{
	public bool NewState { get; }

	public NotEnoughScienceStateChangedEventArgs(bool newState)
	{
		NewState = newState;
	}
}
