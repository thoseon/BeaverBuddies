using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Timberborn.BlueprintSystem;

public class AdvancedDeserializer
{
	private static readonly BindingFlags AllInstanceFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private readonly ImmutableArray<IDeserializer> _customDeserializers;

	public AdvancedDeserializer(IEnumerable<IDeserializer> customDeserializers)
	{
		_customDeserializers = customDeserializers.ToImmutableArray();
	}

	public object Deserialize(object instance, Type type)
	{
		if (_customDeserializers.Length > 0)
		{
			DeserializeInternal(instance, type);
		}
		return instance;
	}

	private void DeserializeInternal(object instance, Type type)
	{
		foreach (PropertyInfo serializedProperty in GetSerializedProperties(type))
		{
			SerializeAttribute customAttribute = serializedProperty.GetCustomAttribute<SerializeAttribute>();
			PropertyInfo property = type.GetProperty(customAttribute.SourceName, AllInstanceFlag);
			if ((object)property != null)
			{
				object value = property.GetValue(instance);
				if (value != null)
				{
					object value2 = DeserializeValue(value, serializedProperty.PropertyType);
					serializedProperty.SetValue(instance, value2);
				}
				continue;
			}
			throw new InvalidOperationException("No source field found for serialized field: " + serializedProperty.Name);
		}
	}

	private static IEnumerable<PropertyInfo> GetSerializedProperties(Type type)
	{
		return from propertyInfo in type.GetProperties(AllInstanceFlag)
			where propertyInfo.GetCustomAttribute<SerializeAttribute>()?.HasSource ?? false
			select propertyInfo;
	}

	private object DeserializeValue(object sourceValue, Type type)
	{
		if (!IsImmutableArray(type))
		{
			return DeserializeSingle(sourceValue, type);
		}
		return DeserializeArray(sourceValue, type);
	}

	private static bool IsImmutableArray(Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(ImmutableArray<>);
		}
		return false;
	}

	private object DeserializeArray(object sourceValue, Type type)
	{
		foreach (IDeserializer customDeserializer in _customDeserializers)
		{
			if (customDeserializer.DeserializedType == type)
			{
				Array array = (Array)sourceValue;
				Array array2 = Array.CreateInstance(type, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					array2.SetValue(customDeserializer.Deserialize(array.GetValue(i)), i);
				}
				return array2;
			}
		}
		throw new InvalidOperationException("No deserializer found for type: " + type);
	}

	private object DeserializeSingle(object sourceValue, Type type)
	{
		foreach (IDeserializer customDeserializer in _customDeserializers)
		{
			if (customDeserializer.DeserializedType == type)
			{
				return customDeserializer.Deserialize(sourceValue);
			}
		}
		throw new InvalidOperationException("No deserializer found for type: " + type);
	}
}
