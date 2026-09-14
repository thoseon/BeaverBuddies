using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Timberborn.EntitySystem;

public class RegisteredComponentService
{
	private static readonly Type RegisteredComponentType = typeof(IRegisteredComponent);

	private readonly Dictionary<Type, ImmutableArray<Type>> _registerableTypes = new Dictionary<Type, ImmutableArray<Type>>();

	public ImmutableArray<Type> GetRegisterableTypes(Type type)
	{
		if (!_registerableTypes.TryGetValue(type, out var value))
		{
			value = FindRegisterableTypes(type);
			_registerableTypes[type] = value;
		}
		return value;
	}

	private static ImmutableArray<Type> FindRegisterableTypes(Type type)
	{
		List<Type> list = new List<Type>();
		Type type2 = type;
		while (type2 != null)
		{
			if (TypeIsRegisteredComponent(type2))
			{
				list.Add(type2);
			}
			type2 = type2.BaseType;
		}
		return list.ToImmutableArray();
	}

	private static bool TypeIsRegisteredComponent(Type baseType)
	{
		return Enumerable.Contains(baseType.GetInterfaces(), RegisteredComponentType);
	}
}
