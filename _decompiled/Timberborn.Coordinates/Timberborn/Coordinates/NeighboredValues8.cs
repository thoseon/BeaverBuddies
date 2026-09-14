using System.Collections.Generic;

namespace Timberborn.Coordinates;

public class NeighboredValues8<T>
{
	private readonly Dictionary<long, OrientedValue<T>> _values = new Dictionary<long, OrientedValue<T>>();

	public void AddVariants(T value, bool down, bool downLeft, bool left, bool upLeft, bool up, bool upRight, bool right, bool downRight)
	{
		_values[GetIndex(down, downLeft, left, upLeft, up, upRight, right, downRight)] = new OrientedValue<T>(value, Orientation.Cw0);
		_values[GetIndex(right, downRight, down, downLeft, left, upLeft, up, upRight)] = new OrientedValue<T>(value, Orientation.Cw90);
		_values[GetIndex(up, upRight, right, downRight, down, downLeft, left, upLeft)] = new OrientedValue<T>(value, Orientation.Cw180);
		_values[GetIndex(left, upLeft, up, upRight, right, downRight, down, downLeft)] = new OrientedValue<T>(value, Orientation.Cw270);
	}

	public void AddExact(T value, bool down, bool downLeft, bool left, bool upLeft, bool up, bool upRight, bool right, bool downRight)
	{
		_values[GetIndex(down, downLeft, left, upLeft, up, upRight, right, downRight)] = new OrientedValue<T>(value, Orientation.Cw0);
	}

	public OrientedValue<T> GetMatch(bool down, bool downLeft, bool left, bool upLeft, bool up, bool upRight, bool right, bool downRight)
	{
		return _values[GetIndex(down, downLeft, left, upLeft, up, upRight, right, downRight)];
	}

	private static int GetIndex(bool down, bool downLeft, bool left, bool upLeft, bool up, bool upRight, bool right, bool downRight)
	{
		return BoolToInt(down) + BoolToInt(downLeft) * 2 + BoolToInt(left) * 4 + BoolToInt(upLeft) * 8 + BoolToInt(up) * 16 + BoolToInt(upRight) * 32 + BoolToInt(right) * 64 + BoolToInt(downRight) * 128;
	}

	private static int BoolToInt(bool value)
	{
		return value ? 1 : 0;
	}
}
