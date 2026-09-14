using System;

namespace Timberborn.ModularShafts;

internal readonly struct FrameVariant : IEquatable<FrameVariant>
{
	public bool Down { get; }

	public bool Left { get; }

	public bool Up { get; }

	public bool Right { get; }

	public bool Bottom { get; }

	public bool Support { get; }

	public FrameVariant(bool down, bool left, bool up, bool right, bool bottom, bool support)
	{
		Down = down;
		Left = left;
		Up = up;
		Right = right;
		Bottom = bottom;
		Support = support;
	}

	public string GetName()
	{
		return Value(Down) + Value(Left) + Value(Up) + Value(Right) + Value(Bottom) + Value(Support);
	}

	public bool Equals(FrameVariant other)
	{
		if (Up == other.Up && Down == other.Down && Left == other.Left && Right == other.Right && Bottom == other.Bottom)
		{
			return Support == other.Support;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is FrameVariant other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Up, Down, Left, Right, Bottom, Support);
	}

	public static bool operator ==(FrameVariant left, FrameVariant right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(FrameVariant left, FrameVariant right)
	{
		return !left.Equals(right);
	}

	private static string Value(bool isSet)
	{
		if (!isSet)
		{
			return "0";
		}
		return "1";
	}
}
