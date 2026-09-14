namespace Timberborn.ReservableSystem;

public struct WorkFinishedEventArgs
{
	public bool WasCompleted { get; }

	public WorkFinishedEventArgs(bool wasCompleted)
	{
		WasCompleted = wasCompleted;
	}
}
