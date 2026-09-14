namespace Timberborn.NotificationSystem;

public class NotificationEventArgs
{
	public Notification Notification { get; }

	public NotificationEventArgs(Notification notification)
	{
		Notification = notification;
	}
}
