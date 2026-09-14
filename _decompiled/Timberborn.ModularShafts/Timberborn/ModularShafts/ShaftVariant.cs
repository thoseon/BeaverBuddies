using System;
using Timberborn.Coordinates;

namespace Timberborn.ModularShafts;

internal readonly struct ShaftVariant : IEquatable<ShaftVariant>
{
	public byte Down { get; }

	public byte Left { get; }

	public byte Up { get; }

	public byte Right { get; }

	public byte Bottom { get; }

	public byte Top { get; }

	public ShaftVariant(byte down, byte left, byte up, byte right, byte bottom, byte top)
	{
		Down = down;
		Left = left;
		Up = up;
		Right = right;
		Bottom = bottom;
		Top = top;
	}

	public static ShaftVariant CreateHorizontal(byte down, byte left, byte up, byte right)
	{
		return new ShaftVariant(down, left, up, right, 0, 0);
	}

	public ShaftVariant ToFacingTop()
	{
		return new ShaftVariant(Down, Left, Up, Right, 0, 1);
	}

	public ShaftVariant ToFacingTopReversed()
	{
		return new ShaftVariant(Down, Left, Up, Right, 0, 2);
	}

	public ShaftVariant ToFacingBottom()
	{
		return new ShaftVariant(Down, Left, Up, Right, 1, 0);
	}

	public ShaftVariant ToFacingBottomReversed()
	{
		return new ShaftVariant(Down, Left, Up, Right, 2, 0);
	}

	public ShaftVariant ToFacingTopAndBottom(bool reverseBottom, bool reverseTop)
	{
		return new ShaftVariant(Down, Left, Up, Right, (byte)((!reverseBottom) ? 1u : 2u), (byte)((!reverseTop) ? 1u : 2u));
	}

	public ShaftVariant Rotate(Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => new ShaftVariant(Down, Left, Up, Right, Bottom, Top), 
			Orientation.Cw90 => new ShaftVariant(Right, Down, Left, Up, Bottom, Top), 
			Orientation.Cw180 => new ShaftVariant(Up, Right, Down, Left, Bottom, Top), 
			Orientation.Cw270 => new ShaftVariant(Left, Up, Right, Down, Bottom, Top), 
			_ => throw new ArgumentOutOfRangeException("orientation", orientation, null), 
		};
	}

	public ShaftVariant AddSymmetryRight()
	{
		return new ShaftVariant(Down, Left, Up, (byte)((Left != 1) ? 1u : 2u), Bottom, Top);
	}

	public ShaftVariant AddSymmetryLeft()
	{
		return new ShaftVariant(Down, (byte)((Right != 1) ? 1u : 2u), Up, Right, Bottom, Top);
	}

	public ShaftVariant AddSymmetryUp()
	{
		return new ShaftVariant((byte)((Down != 1) ? 1u : 2u), Left, Up, Right, Bottom, Top);
	}

	public ShaftVariant AddSymmetryDown()
	{
		return new ShaftVariant(Down, Left, (byte)((Up != 1) ? 1u : 2u), Right, Bottom, Top);
	}

	public byte GetRotation(Direction3D direction)
	{
		return direction switch
		{
			Direction3D.Up => Up, 
			Direction3D.Down => Down, 
			Direction3D.Left => Left, 
			Direction3D.Right => Right, 
			Direction3D.Top => Top, 
			Direction3D.Bottom => Bottom, 
			_ => throw new ArgumentOutOfRangeException("direction", direction, null), 
		};
	}

	public string GetName()
	{
		return Value(Down) + Value(Left) + Value(Up) + Value(Right) + Value(Bottom) + Value(Top);
	}

	public bool Equals(ShaftVariant other)
	{
		if (Up == other.Up && Down == other.Down && Left == other.Left && Right == other.Right && Bottom == other.Bottom)
		{
			return Top == other.Top;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is ShaftVariant other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Up, Down, Left, Right, Bottom, Top);
	}

	public static bool operator ==(ShaftVariant left, ShaftVariant right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ShaftVariant left, ShaftVariant right)
	{
		return !left.Equals(right);
	}

	private static string Value(byte state)
	{
		return state.ToString();
	}
}
