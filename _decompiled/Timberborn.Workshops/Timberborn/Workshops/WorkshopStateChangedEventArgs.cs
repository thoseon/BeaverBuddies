namespace Timberborn.Workshops;

public class WorkshopStateChangedEventArgs
{
	public bool CurrentlyProducing { get; }

	public WorkshopStateChangedEventArgs(bool currentlyProducing)
	{
		CurrentlyProducing = currentlyProducing;
	}
}
