using System;
using System.Collections.Generic;

namespace Timberborn.Common;

public readonly struct NullableKey<T> : IEquatable<NullableKey<T>> where T : class
{
	public T Key { get; }

	public NullableKey(T key)
	{
		Key = key;
	}

	public bool Equals(NullableKey<T> other)
	{
		return EqualityComparer<T>.Default.Equals(Key, other.Key);
	}

	public override bool Equals(object obj)
	{
		if (obj is NullableKey<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return EqualityComparer<T>.Default.GetHashCode(Key);
	}
}
