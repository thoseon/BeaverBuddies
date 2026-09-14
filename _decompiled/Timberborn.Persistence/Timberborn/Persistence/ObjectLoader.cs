using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.Persistence;

public class ObjectLoader : IObjectLoader
{
	private readonly SerializedObject _serializedObject;

	public ObjectLoader(SerializedObject serializedObject)
	{
		_serializedObject = serializedObject;
	}

	public int Get(PropertyKey<int> key)
	{
		return _serializedObject.Get<int>(key.Name);
	}

	public float Get(PropertyKey<float> key)
	{
		return _serializedObject.Get<float>(key.Name);
	}

	public bool Get(PropertyKey<bool> key)
	{
		return _serializedObject.Get<bool>(key.Name);
	}

	public string Get(PropertyKey<string> key)
	{
		return _serializedObject.Get<string>(key.Name);
	}

	public char Get(PropertyKey<char> key)
	{
		return _serializedObject.Get<char>(key.Name);
	}

	public Quaternion Get(PropertyKey<Quaternion> key)
	{
		return _serializedObject.Get<Quaternion>(key.Name);
	}

	public Vector3 Get(PropertyKey<Vector3> key)
	{
		return _serializedObject.Get<Vector3>(key.Name);
	}

	public Vector3Int Get(PropertyKey<Vector3Int> key)
	{
		return _serializedObject.Get<Vector3Int>(key.Name);
	}

	public Vector2 Get(PropertyKey<Vector2> key)
	{
		return _serializedObject.Get<Vector2>(key.Name);
	}

	public Vector2Int Get(PropertyKey<Vector2Int> key)
	{
		return _serializedObject.Get<Vector2Int>(key.Name);
	}

	public Color Get(PropertyKey<Color> key)
	{
		return _serializedObject.Get<Color>(key.Name);
	}

	public Guid Get(PropertyKey<Guid> key)
	{
		return _serializedObject.Get<Guid>(key.Name);
	}

	public T Get<T>(PropertyKey<T> key) where T : Enum
	{
		return _serializedObject.Get<T>(key.Name);
	}

	public T Get<T>(PropertyKey<T> key, IValueSerializer<T> serializer)
	{
		if (GetObsoletable(key, serializer, out var value))
		{
			return value;
		}
		throw new ObsoleteValueException($"Couldn't deconvert {key.Name} to {typeof(T)}");
	}

	public bool GetObsoletable<T>(PropertyKey<T> key, IValueSerializer<T> serializer, out T value)
	{
		return SaveConversions.Deconvert(_serializedObject.GetSerialized(key.Name), serializer, out value);
	}

	public List<int> Get(ListKey<int> key)
	{
		return _serializedObject.GetArray<int>(key.Name).ToList();
	}

	public List<float> Get(ListKey<float> key)
	{
		return _serializedObject.GetArray<float>(key.Name).ToList();
	}

	public List<bool> Get(ListKey<bool> key)
	{
		return _serializedObject.GetArray<bool>(key.Name).ToList();
	}

	public List<string> Get(ListKey<string> key)
	{
		return _serializedObject.GetArray<string>(key.Name).ToList();
	}

	public List<char> Get(ListKey<char> key)
	{
		return _serializedObject.GetArray<char>(key.Name).ToList();
	}

	public List<Quaternion> Get(ListKey<Quaternion> key)
	{
		return _serializedObject.GetArray<Quaternion>(key.Name).ToList();
	}

	public List<Vector3> Get(ListKey<Vector3> key)
	{
		return _serializedObject.GetArray<Vector3>(key.Name).ToList();
	}

	public List<Vector3Int> Get(ListKey<Vector3Int> key)
	{
		return _serializedObject.GetArray<Vector3Int>(key.Name).ToList();
	}

	public List<Vector2> Get(ListKey<Vector2> key)
	{
		return _serializedObject.GetArray<Vector2>(key.Name).ToList();
	}

	public List<Vector2Int> Get(ListKey<Vector2Int> key)
	{
		return _serializedObject.GetArray<Vector2Int>(key.Name).ToList();
	}

	public List<Color> Get(ListKey<Color> key)
	{
		return _serializedObject.GetArray<Color>(key.Name).ToList();
	}

	public List<Guid> Get(ListKey<Guid> key)
	{
		return _serializedObject.GetArray<Guid>(key.Name).ToList();
	}

	public List<T> Get<T>(ListKey<T> key) where T : Enum
	{
		return _serializedObject.GetArray<T>(key.Name).ToList();
	}

	public List<T> Get<T>(ListKey<T> key, IValueSerializer<T> serializer)
	{
		return SaveConversions.DeconvertList(serializer, (object[])_serializedObject.GetSerialized(key.Name));
	}

	public bool Has<T>(PropertyKey<T> key)
	{
		return Has(key.Name);
	}

	public bool Has<T>(ListKey<T> key)
	{
		return Has(key.Name);
	}

	private bool Has(string name)
	{
		return _serializedObject.Has(name);
	}
}
