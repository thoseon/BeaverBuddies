using UnityEngine.InputSystem.LowLevel;

namespace Timberborn.KeyBindingSystem;

public static class InputEventPtrExtensions
{
	public static bool IsAnyStateEvent(this InputEventPtr inputEvent)
	{
		if (!inputEvent.IsA<StateEvent>())
		{
			return inputEvent.IsA<DeltaStateEvent>();
		}
		return true;
	}
}
