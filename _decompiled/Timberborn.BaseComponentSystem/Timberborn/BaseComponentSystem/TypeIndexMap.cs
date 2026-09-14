using System;
using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.BaseComponentSystem;

internal class TypeIndexMap
{
	private readonly Dictionary<Type, object> _typeIndex = new Dictionary<Type, object>();

	public object GetIndex(Type requested)
	{
		return _typeIndex[requested];
	}

	public bool TryGetIndex(Type requested, out object index)
	{
		return _typeIndex.TryGetValue(requested, out index);
	}

	public void CacheType<T>(ReadOnlyList<object> components)
	{
		Type typeFromHandle = typeof(T);
		int num = 0;
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] is T)
			{
				num++;
				CacheComponent(num, typeFromHandle, i);
			}
		}
		if (num == 0)
		{
			_typeIndex[typeFromHandle] = null;
		}
	}

	private void CacheComponent(int count, Type key, int index)
	{
		switch (count)
		{
		case 1:
			_typeIndex[key] = index;
			break;
		case 2:
		{
			List<int> value = new List<int>
			{
				(int)_typeIndex[key],
				index
			};
			_typeIndex[key] = value;
			break;
		}
		default:
			((List<int>)_typeIndex[key]).Add(index);
			break;
		}
	}
}
