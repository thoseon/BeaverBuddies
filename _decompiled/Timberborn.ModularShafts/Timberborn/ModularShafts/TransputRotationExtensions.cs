using System;

namespace Timberborn.ModularShafts;

internal static class TransputRotationExtensions
{
	public static TransputRotation ReverseOrSetNormal(this TransputRotation rotation)
	{
		return rotation switch
		{
			TransputRotation.None => TransputRotation.Normal, 
			TransputRotation.Normal => TransputRotation.Reversed, 
			TransputRotation.Reversed => TransputRotation.Normal, 
			TransputRotation.Ignored => TransputRotation.Normal, 
			_ => throw new ArgumentOutOfRangeException("rotation", rotation, null), 
		};
	}

	public static byte AsByte(this TransputRotation rotation)
	{
		return rotation switch
		{
			TransputRotation.None => 0, 
			TransputRotation.Normal => 1, 
			TransputRotation.Reversed => 2, 
			TransputRotation.Ignored => 0, 
			_ => throw new ArgumentOutOfRangeException("rotation", rotation, null), 
		};
	}
}
