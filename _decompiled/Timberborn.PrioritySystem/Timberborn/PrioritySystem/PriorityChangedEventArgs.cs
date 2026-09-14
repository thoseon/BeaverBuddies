namespace Timberborn.PrioritySystem;

public class PriorityChangedEventArgs
{
	public Priority PreviousPriority { get; }

	public PriorityChangedEventArgs(Priority previousPriority)
	{
		PreviousPriority = previousPriority;
	}
}
