namespace Timberborn.InputSystem;

public readonly struct KeyPressedEvent
{
	public string Key { get; }

	public KeyPressedEvent(string key)
	{
		Key = key;
	}
}
