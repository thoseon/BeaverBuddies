using System;

namespace Timberborn.QuickNotificationSystem;

public class QuickNotificationService
{
	public event EventHandler<QuickNotificationEventArgs> AlertSent;

	public void SendNotification(string text)
	{
		SendNotification(text, isWaring: false);
	}

	public void SendWarningNotification(string text)
	{
		SendNotification(text, isWaring: true);
	}

	private void SendNotification(string text, bool isWaring)
	{
		AlertSent?.Invoke(this, new QuickNotificationEventArgs(text, isWaring));
	}
}
