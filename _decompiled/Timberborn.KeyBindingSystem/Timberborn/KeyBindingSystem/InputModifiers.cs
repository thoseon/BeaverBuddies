using System;

namespace Timberborn.KeyBindingSystem;

[Flags]
public enum InputModifiers
{
	None = 0,
	Ctrl = 1,
	Alt = 2,
	Shift = 4,
	Cmd = 8
}
