namespace Timberborn.StatusSystem;

public class NotifyingStatusChangedEvent
{
	public string StatusAlert { get; }

	public NotifyingStatusChangedEvent(string statusAlert)
	{
		StatusAlert = statusAlert;
	}
}
