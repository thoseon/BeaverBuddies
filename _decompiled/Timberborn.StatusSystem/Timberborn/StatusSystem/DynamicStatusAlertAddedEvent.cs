namespace Timberborn.StatusSystem;

public class DynamicStatusAlertAddedEvent
{
	public StatusInstance StatusInstance { get; }

	public DynamicStatusAlertAddedEvent(StatusInstance statusInstance)
	{
		StatusInstance = statusInstance;
	}
}
