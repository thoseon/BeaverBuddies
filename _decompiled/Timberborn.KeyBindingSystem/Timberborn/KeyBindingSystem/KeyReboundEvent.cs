namespace Timberborn.KeyBindingSystem;

public class KeyReboundEvent
{
	public string KeyBindingId { get; }

	public KeyReboundEvent(string keyBindingId)
	{
		KeyBindingId = keyBindingId;
	}
}
