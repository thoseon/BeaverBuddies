using System;
using System.Collections.Generic;

namespace Timberborn.SerializationSystem;

public class SerializedObject : IEquatable<SerializedObject>
{
	private readonly Dictionary<string, object> _properties;

	public SerializedObject()
	{
		_properties = new Dictionary<string, object>();
	}

	public SerializedObject(Dictionary<string, object> properties)
	{
		_properties = properties;
	}

	public void Set<T>(string name, T value)
	{
		_properties[name] = PrimitiveTypeSerialization.Serialize(value);
	}

	public void SetArray(string name, Array values)
	{
		object[] array = new object[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = PrimitiveTypeSerialization.Serialize(values.GetValue(i));
		}
		_properties[name] = array;
	}

	public T Get<T>(string name)
	{
		return (T)Get(name, typeof(T));
	}

	public object Get(string name, Type type)
	{
		if (TryGet(name, type, out var value))
		{
			return value;
		}
		throw new ArgumentOutOfRangeException("name", "Property not found: '" + name + "'");
	}

	public T GetOrDefault<T>(string name, T defaultValue)
	{
		if (TryGet(name, typeof(T), out var value))
		{
			return (T)value;
		}
		return defaultValue;
	}

	public bool TryGet(string name, Type type, out object value)
	{
		if (_properties.TryGetValue(name, out var value2))
		{
			value = PrimitiveTypeSerialization.Deserialize(value2, type);
			return true;
		}
		value = null;
		return false;
	}

	public object GetSerialized(string name)
	{
		if (_properties.TryGetValue(name, out var value))
		{
			return value;
		}
		throw new ArgumentOutOfRangeException("name", "Property not found: '" + name + "'");
	}

	public T[] GetArray<T>(string name)
	{
		if (TryGetArray(name, typeof(T), out var array))
		{
			return (T[])array;
		}
		throw new ArgumentOutOfRangeException("name", "Property not found: '" + name + "'");
	}

	public bool TryGetArray(string name, Type type, out Array array)
	{
		if (_properties.TryGetValue(name, out var value))
		{
			object[] array2 = (object[])value;
			array = Array.CreateInstance(type, array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				object value2 = PrimitiveTypeSerialization.Deserialize(array2[i], type);
				array.SetValue(value2, i);
			}
			return true;
		}
		array = null;
		return false;
	}

	public bool Has(string name)
	{
		return _properties.ContainsKey(name);
	}

	public IEnumerable<string> Properties()
	{
		return _properties.Keys;
	}

	public bool Equals(SerializedObject other)
	{
		if (other == null)
		{
			return false;
		}
		if (_properties.Count != other._properties.Count)
		{
			return false;
		}
		foreach (KeyValuePair<string, object> property in _properties)
		{
			if (other._properties.TryGetValue(property.Key, out var value))
			{
				if (value is SerializedObject serializedObject)
				{
					if (!serializedObject.Equals((SerializedObject)property.Value))
					{
						return false;
					}
				}
				else if (!value.Equals(property.Value))
				{
					return false;
				}
				continue;
			}
			return false;
		}
		return true;
	}
}
