using System;

namespace Timberborn.InputSystem;

[Flags]
public enum ScreenEdges
{
	None = 0,
	Down = 1,
	Left = 2,
	Up = 4,
	Right = 8
}
