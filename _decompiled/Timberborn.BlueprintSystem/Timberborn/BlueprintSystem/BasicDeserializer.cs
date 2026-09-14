using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Timberborn.SerializationSystem;

namespace Timberborn.BlueprintSystem;

public class BasicDeserializer
{
	private readonly AssetRefDeserializer _assetRefDeserializer;

	public BasicDeserializer(AssetRefDeserializer assetRefDeserializer)
	{
		_assetRefDeserializer = assetRefDeserializer;
	}

	public object Deserialize(SerializedObject serializedObject, Type type)
	{
		object obj = Activator.CreateInstance(type);
		foreach (PropertyInfo serializedProperty in type.GetSerializedProperties())
		{
			object value = GetValue(serializedObject, serializedProperty);
			if (value != null)
			{
				serializedProperty.SetValue(obj, value);
			}
		}
		return obj;
	}

	private object GetValue(SerializedObject serializedObject, PropertyInfo serializedProperty)
	{
		if (!IsImmutableArray(serializedProperty.PropertyType))
		{
			return DeserializeSingle(serializedObject, serializedProperty);
		}
		return DeserializeArray(serializedObject, serializedProperty);
	}

	private static bool IsImmutableArray(Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(ImmutableArray<>);
		}
		return false;
	}

	private object DeserializeArray(SerializedObject serializedObject, PropertyInfo serializedProperty)
	{
		string name = serializedProperty.Name;
		Type type = serializedProperty.PropertyType.GenericTypeArguments[0];
		if (_assetRefDeserializer.TryDeserializeArray(serializedObject, name, type, out var assetArray))
		{
			return AsImmutableArray(assetArray, type);
		}
		if (type.IsSerializable() && serializedObject.Has(name))
		{
			return AsImmutableArray(DeserializeArray(serializedObject, name, type), type);
		}
		if (serializedObject.TryGetArray(name, type, out var array))
		{
			return AsImmutableArray(array, type);
		}
		return AsImmutableArray(Array.CreateInstance(type, 0), type);
	}

	private object DeserializeSingle(SerializedObject serializedObject, PropertyInfo serializedProperty)
	{
		string name = serializedProperty.Name;
		Type propertyType = serializedProperty.PropertyType;
		if (_assetRefDeserializer.TryDeserialize(serializedObject, name, propertyType, out var asset))
		{
			return asset;
		}
		if (propertyType.IsSerializable() && serializedObject.Has(name))
		{
			return Deserialize(serializedObject.Get<SerializedObject>(name), propertyType);
		}
		Type type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
		if (serializedObject.TryGet(name, type, out var value))
		{
			return value;
		}
		return null;
	}

	private Array DeserializeArray(SerializedObject serializedObject, string name, Type type)
	{
		SerializedObject[] array = serializedObject.GetArray<SerializedObject>(name);
		Array array2 = Array.CreateInstance(type, array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			array2.SetValue(Deserialize(array[i], type), i);
		}
		return array2;
	}

	private static object AsImmutableArray(Array array, Type type)
	{
		return typeof(ImmutableArray<>).MakeGenericType(type).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single()
			.Invoke(new object[1] { array });
	}
}
