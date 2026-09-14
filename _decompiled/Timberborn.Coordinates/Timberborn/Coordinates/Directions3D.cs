using System;

namespace Timberborn.Coordinates;

[Flags]
public enum Directions3D
{
	None = 0,
	All = -1,
	Down = 1,
	Left = 2,
	Up = 4,
	Right = 8,
	Bottom = 0x10,
	Top = 0x20
}
