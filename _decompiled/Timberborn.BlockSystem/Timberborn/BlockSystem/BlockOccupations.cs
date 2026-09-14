using System;

namespace Timberborn.BlockSystem;

[Flags]
public enum BlockOccupations
{
	None = 0,
	All = -1,
	Floor = 1,
	Bottom = 2,
	Top = 4,
	Corners = 8,
	Path = 0x10,
	Middle = 0x20
}
