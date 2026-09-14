namespace Timberborn.QuickNotificationSystem;

public class QuickNotificationEventArgs
{
	public string Text { get; }

	public bool IsWarning { get; }

	public QuickNotificationEventArgs(string text, bool isWarning)
	{
		Text = text;
		IsWarning = isWarning;
	}
}
