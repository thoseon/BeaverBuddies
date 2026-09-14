using System;
using Timberborn.Coordinates;

namespace Timberborn.WaterSystem;

public static class FlowDirectionExtensions
{
	public static FlowDirection ToFlowDirection(this Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => FlowDirection.Top, 
			Orientation.Cw90 => FlowDirection.Right, 
			Orientation.Cw180 => FlowDirection.Bottom, 
			Orientation.Cw270 => FlowDirection.Left, 
			_ => throw new ArgumentOutOfRangeException("orientation", orientation, null), 
		};
	}
}
