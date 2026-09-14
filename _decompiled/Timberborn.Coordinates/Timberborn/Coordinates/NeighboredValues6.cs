using System;
using System.Collections.Generic;

namespace Timberborn.Coordinates;

public class NeighboredValues6<T>
{
	private readonly Dictionary<long, OrientedValue<T>> _values = new Dictionary<long, OrientedValue<T>>();

	public void AddVariants(T value, bool down, bool left, bool up, bool right, bool top, bool bottom)
	{
		AddVariants(value, BoolToByte(down), BoolToByte(left), BoolToByte(up), BoolToByte(right), BoolToByte(top), BoolToByte(bottom));
	}

	public OrientedValue<T> GetMatch(bool down, bool left, bool up, bool right, bool top, bool bottom)
	{
		if (TryGetMatch(BoolToByte(down), BoolToByte(left), BoolToByte(up), BoolToByte(right), BoolToByte(top), BoolToByte(bottom), out var value))
		{
			return value;
		}
		throw new ArgumentOutOfRangeException($"Couldn't find value for {down} {left} {up} {right} {top} {bottom}");
	}

	private bool TryGetMatch(byte down, byte left, byte up, byte right, byte top, byte bottom, out OrientedValue<T> value)
	{
		return _values.TryGetValue(GetIndex(down, left, up, right, top, bottom), out value);
	}

	private void AddVariants(T value, byte down, byte left, byte up, byte right, byte top, byte bottom)
	{
		_values[GetIndex(down, left, up, right, top, bottom)] = new OrientedValue<T>(value, Orientation.Cw0);
		_values[GetIndex(right, down, left, up, top, bottom)] = new OrientedValue<T>(value, Orientation.Cw90);
		_values[GetIndex(up, right, down, left, top, bottom)] = new OrientedValue<T>(value, Orientation.Cw180);
		_values[GetIndex(left, up, right, down, top, bottom)] = new OrientedValue<T>(value, Orientation.Cw270);
	}

	private static byte BoolToByte(bool key)
	{
		return (!key) ? ((byte)1) : ((byte)0);
	}

	private static long GetIndex(byte down, byte left, byte up, byte right, byte top, byte bottom)
	{
		return down + (long)left * 256L + (long)up * 256L * 256 + (long)right * 256L * 256 * 256 + (long)top * 256L * 256 * 256 * 256 + (long)bottom * 256L * 256 * 256 * 256 * 256;
	}
}
