namespace Timberborn.KeyBindingSystem;

public interface IKeyBindingBlocker
{
	bool IsKeyBlocked(KeyBinding keyBinding);
}
