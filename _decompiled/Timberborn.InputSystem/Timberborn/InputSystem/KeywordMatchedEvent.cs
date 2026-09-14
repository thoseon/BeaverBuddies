namespace Timberborn.InputSystem;

public class KeywordMatchedEvent
{
	public string KeywordNotification { get; }

	public KeywordMatchedEvent(string keywordNotification)
	{
		KeywordNotification = keywordNotification;
	}
}
