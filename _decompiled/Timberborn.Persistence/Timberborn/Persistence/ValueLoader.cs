using System;
using System.Collections.Generic;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.Persistence;

public class ValueLoader : IValueLoader
{
	private readonly object _value;

	public ValueLoader(object value)
	{
		_value = value;
	}

	public int AsInt()
	{
		return AsPrimitive<int>();
	}

	public float AsFloat()
	{
		return AsPrimitive<float>();
	}

	public bool AsBool()
	{
		return AsPrimitive<bool>();
	}

	public string AsString()
	{
		return AsPrimitive<string>();
	}

	public char AsChar()
	{
		return AsPrimitive<char>();
	}

	public Quaternion AsQuaternion()
	{
		return AsPrimitive<Quaternion>();
	}

	public Vector3 AsVector3()
	{
		return AsPrimitive<Vector3>();
	}

	public Vector3Int AsVector3Int()
	{
		return AsPrimitive<Vector3Int>();
	}

	public Vector2 AsVector2()
	{
		return AsPrimitive<Vector2>();
	}

	public Vector2Int AsVector2Int()
	{
		return AsPrimitive<Vector2Int>();
	}

	public Color AsColor()
	{
		return AsPrimitive<Color>();
	}

	public Guid AsGuid()
	{
		return AsPrimitive<Guid>();
	}

	public T As<T>(IValueSerializer<T> serializer)
	{
		if (AsObsoletable(serializer, out var value))
		{
			return value;
		}
		throw new ObsoleteValueException($"Couldn't deconvert to {typeof(T)}");
	}

	public bool AsObsoletable<T>(IValueSerializer<T> serializer, out T value)
	{
		return SaveConversions.Deconvert(AsPrimitive<SerializedObject>(), serializer, out value);
	}

	public List<int> AsIntList()
	{
		return AsPrimitiveList<int>();
	}

	public List<float> AsFloatList()
	{
		return AsPrimitiveList<float>();
	}

	public List<bool> AsBoolList()
	{
		return AsPrimitiveList<bool>();
	}

	public List<string> AsStringList()
	{
		return AsPrimitiveList<string>();
	}

	public List<char> AsCharList()
	{
		return AsPrimitiveList<char>();
	}

	public List<Quaternion> AsQuaternionList()
	{
		return AsPrimitiveList<Quaternion>();
	}

	public List<Vector3> AsVector3List()
	{
		return AsPrimitiveList<Vector3>();
	}

	public List<Vector3Int> AsVector3IntList()
	{
		return AsPrimitiveList<Vector3Int>();
	}

	public List<Vector2> AsVector2List()
	{
		return AsPrimitiveList<Vector2>();
	}

	public List<Vector2Int> AsVector2IntList()
	{
		return AsPrimitiveList<Vector2Int>();
	}

	public List<Color> AsColorList()
	{
		return AsPrimitiveList<Color>();
	}

	public List<Guid> AsGuidList()
	{
		return AsPrimitiveList<Guid>();
	}

	public List<T> AsList<T>(IValueSerializer<T> serializer)
	{
		return SaveConversions.DeconvertList(serializer, AsPrimitiveList<SerializedObject>());
	}

	public IObjectLoader AsObject()
	{
		return new ObjectLoader((SerializedObject)_value);
	}

	public bool IsObject()
	{
		return _value is SerializedObject;
	}

	public bool IsList()
	{
		return _value is object[];
	}

	private T AsPrimitive<T>()
	{
		return (T)PrimitiveTypeSerialization.Deserialize(_value, typeof(T));
	}

	private List<T> AsPrimitiveList<T>()
	{
		object[] array = (object[])_value;
		List<T> list = new List<T>(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			list.Add((T)PrimitiveTypeSerialization.Deserialize(array[i], typeof(T)));
		}
		return list;
	}
}
