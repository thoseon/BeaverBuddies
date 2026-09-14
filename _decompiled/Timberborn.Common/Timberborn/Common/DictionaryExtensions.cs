using System;
using System.Collections.Generic;

namespace Timberborn.Common;

public static class DictionaryExtensions
{
	public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> supplier)
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		value = supplier();
		dictionary.Add(key, value);
		return value;
	}

	public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
	{
		return dictionary.GetOrAdd(key, () => new TValue());
	}

	public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
	{
		if (!dictionary.TryGetValue(key, out var value))
		{
			return default(TValue);
		}
		return value;
	}
}
