using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.Persistence;

public class ObjectSaver : IObjectSaver
{
	private readonly SerializedObject _serializedObject;

	public ObjectSaver(SerializedObject serializedObject)
	{
		_serializedObject = serializedObject;
	}

	public void Set(PropertyKey<int> key, int value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<float> key, float value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<bool> key, bool value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<string> key, string value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<char> key, char value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<Quaternion> key, Quaternion value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<Vector3> key, Vector3 value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<Vector3Int> key, Vector3Int value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<Vector2> key, Vector2 value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<Vector2Int> key, Vector2Int value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<Color> key, Color value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set(PropertyKey<Guid> key, Guid value)
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set<T>(PropertyKey<T> key, T value) where T : Enum
	{
		_serializedObject.Set(key.Name, value);
	}

	public void Set<T>(PropertyKey<T> key, T value, IValueSerializer<T> serializer)
	{
		_serializedObject.Set(key.Name, SaveConversions.Convert(value, serializer));
	}

	public void Set(ListKey<int> key, IReadOnlyCollection<int> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<float> key, IReadOnlyCollection<float> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<bool> key, IReadOnlyCollection<bool> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<string> key, IReadOnlyCollection<string> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<char> key, IReadOnlyCollection<char> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<Quaternion> key, IReadOnlyCollection<Quaternion> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<Vector3> key, IReadOnlyCollection<Vector3> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<Vector3Int> key, IReadOnlyCollection<Vector3Int> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<Vector2> key, IReadOnlyCollection<Vector2> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<Vector2Int> key, IReadOnlyCollection<Vector2Int> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<Color> key, IReadOnlyCollection<Color> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set(ListKey<Guid> key, IReadOnlyCollection<Guid> values)
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set<T>(ListKey<T> key, IReadOnlyCollection<T> values) where T : Enum
	{
		_serializedObject.SetArray(key.Name, values.ToArray());
	}

	public void Set<T>(ListKey<T> key, IReadOnlyCollection<T> values, IValueSerializer<T> serializer)
	{
		_serializedObject.SetArray(key.Name, SaveConversions.ConvertList(values, serializer));
	}
}
