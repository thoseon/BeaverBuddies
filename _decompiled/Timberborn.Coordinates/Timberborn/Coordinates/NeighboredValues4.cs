using System;
using System.Collections.Generic;

namespace Timberborn.Coordinates;

public class NeighboredValues4<T>
{
	private readonly Dictionary<long, OrientedValue<T>> _values = new Dictionary<long, OrientedValue<T>>();

	public bool IsEmpty => _values.Count == 0;

	public void AddVariants(T value, bool down, bool left, bool up, bool right)
	{
		AddVariants(value, BoolToByte(down), BoolToByte(left), BoolToByte(up), BoolToByte(right));
	}

	public void AddExact(T value, byte down, byte left, byte up, byte right)
	{
		_values[GetIndex(down, left, up, right)] = new OrientedValue<T>(value, Orientation.Cw0);
	}

	public OrientedValue<T> GetMatch(bool down, bool left, bool up, bool right)
	{
		if (TryGetMatch(BoolToByte(down), BoolToByte(left), BoolToByte(up), BoolToByte(right), out var value))
		{
			return value;
		}
		throw new ArgumentOutOfRangeException($"Couldn't find value for {down} {left} {up} {right}");
	}

	public bool TryGetMatch(byte down, byte left, byte up, byte right, out OrientedValue<T> value)
	{
		return _values.TryGetValue(GetIndex(down, left, up, right), out value);
	}

	private void AddVariants(T value, byte down, byte left, byte up, byte right)
	{
		_values[GetIndex(down, left, up, right)] = new OrientedValue<T>(value, Orientation.Cw0);
		_values[GetIndex(right, down, left, up)] = new OrientedValue<T>(value, Orientation.Cw90);
		_values[GetIndex(up, right, down, left)] = new OrientedValue<T>(value, Orientation.Cw180);
		_values[GetIndex(left, up, right, down)] = new OrientedValue<T>(value, Orientation.Cw270);
	}

	private static byte BoolToByte(bool key)
	{
		return (!key) ? ((byte)1) : ((byte)0);
	}

	private static long GetIndex(byte down, byte left, byte up, byte right)
	{
		return down + (long)left * 256L + (long)up * 256L * 256 + (long)right * 256L * 256 * 256;
	}
}
