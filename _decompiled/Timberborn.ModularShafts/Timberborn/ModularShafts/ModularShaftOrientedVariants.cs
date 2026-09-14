using System;
using System.Collections.Generic;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ModularShaftOrientedVariants
{
	private readonly Dictionary<long, OrientedValue<GameObject>> _values = new Dictionary<long, OrientedValue<GameObject>>();

	public bool Contains(ShaftVariant variant)
	{
		return _values.ContainsKey(GetIndex(variant));
	}

	public void AddVariant(GameObject value, ShaftVariant variant)
	{
		_values[GetIndex(variant)] = new OrientedValue<GameObject>(value, Orientation.Cw0);
		_values[GetIndex(variant.Rotate(Orientation.Cw90))] = new OrientedValue<GameObject>(value, Orientation.Cw90);
		_values[GetIndex(variant.Rotate(Orientation.Cw180))] = new OrientedValue<GameObject>(value, Orientation.Cw180);
		_values[GetIndex(variant.Rotate(Orientation.Cw270))] = new OrientedValue<GameObject>(value, Orientation.Cw270);
	}

	public OrientedValue<GameObject> GetMatch(ShaftVariant variant)
	{
		if (_values.TryGetValue(GetIndex(variant), out var value))
		{
			return value;
		}
		throw new ArgumentOutOfRangeException("Couldn't find value for " + variant.GetName());
	}

	private static long GetIndex(ShaftVariant variant)
	{
		return (long)(variant.Down | ((ulong)variant.Left << 8) | ((ulong)variant.Up << 16) | ((ulong)variant.Right << 24) | ((ulong)variant.Bottom << 32) | ((ulong)variant.Top << 40));
	}
}
