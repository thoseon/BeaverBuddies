using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Timberborn.BlueprintSystem;

public class SpecTypeCache
{
	private static readonly string ChildrenProperty = "Children";

	private static readonly Type SpecType = typeof(ComponentSpec);

	private static readonly Type SpecAliasAttribute = typeof(SpecAliasAttribute);

	private readonly FrozenDictionary<string, Type> _typeMap;

	private SpecTypeCache(FrozenDictionary<string, Type> typeMap)
	{
		_typeMap = typeMap;
	}

	public static SpecTypeCache Create()
	{
		return new SpecTypeCache(AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetTypes()).Where(IsSpecClass)
			.SelectMany(GetSpecEntries)
			.ToFrozenDictionary());
	}

	public bool TryGetType(string key, out Type type)
	{
		if (_typeMap.TryGetValue(key, out type))
		{
			return true;
		}
		if (key != ChildrenProperty)
		{
			Debug.LogWarning("No type found for key " + key);
		}
		return false;
	}

	private static bool IsSpecClass(Type type)
	{
		if (type.IsClass && !type.IsAbstract)
		{
			return SpecType.IsAssignableFrom(type);
		}
		return false;
	}

	private static IEnumerable<KeyValuePair<string, Type>> GetSpecEntries(Type type)
	{
		if (type.GetCustomAttributes(SpecAliasAttribute, inherit: true).SingleOrDefault() is SpecAliasAttribute { Aliases: var aliases })
		{
			foreach (string item in aliases)
			{
				yield return new KeyValuePair<string, Type>(item, type);
			}
		}
		yield return new KeyValuePair<string, Type>(type.Name, type);
	}
}
