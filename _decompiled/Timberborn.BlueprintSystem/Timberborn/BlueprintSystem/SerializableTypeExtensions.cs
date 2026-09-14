using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Timberborn.BlueprintSystem;

public static class SerializableTypeExtensions
{
	private static readonly BindingFlags AllInstanceFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public static IEnumerable<PropertyInfo> GetSerializedProperties(this Type type)
	{
		return from property in type.GetProperties(AllInstanceFlag).Where(delegate(PropertyInfo propertyInfo)
			{
				SerializeAttribute customAttribute = propertyInfo.GetCustomAttribute<SerializeAttribute>();
				return customAttribute != null && !customAttribute.HasSource;
			})
			orderby (!(property.DeclaringType != type)) ? 1 : 0
			select property;
	}

	public static bool IsSerializable(this Type type)
	{
		if (!typeof(ComponentSpec).IsAssignableFrom(type))
		{
			return type.GetProperties(AllInstanceFlag).Any((PropertyInfo propertyInfo) => propertyInfo.GetCustomAttribute<SerializeAttribute>() != null);
		}
		return true;
	}
}
