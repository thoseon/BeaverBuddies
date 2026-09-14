using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.Persistence;

public class ValueSaver : IValueSaver
{
	public object Value { get; private set; }

	public void AsInt(int value)
	{
		Value = value;
	}

	public void AsFloat(float value)
	{
		Value = value;
	}

	public void AsBool(bool value)
	{
		Value = value;
	}

	public void AsString(string value)
	{
		Value = value;
	}

	public void AsChar(char value)
	{
		Value = value;
	}

	public void AsQuaternion(Quaternion value)
	{
		Value = value;
	}

	public void AsVector3(Vector3 value)
	{
		Value = value;
	}

	public void AsVector3Int(Vector3Int value)
	{
		Value = value;
	}

	public void AsVector2(Vector2 value)
	{
		Value = value;
	}

	public void AsVector2Int(Vector2Int value)
	{
		Value = value;
	}

	public void AsColor(Color value)
	{
		Value = value;
	}

	public void AsGuid(Guid value)
	{
		Value = value;
	}

	public void As<T>(T value, IValueSerializer<T> serializer)
	{
		Value = SaveConversions.Convert(value, serializer);
	}

	public void AsIntList(IReadOnlyCollection<int> values)
	{
		Value = values.ToArray();
	}

	public void AsFloatList(IReadOnlyCollection<float> values)
	{
		Value = values.ToArray();
	}

	public void AsBoolList(IReadOnlyCollection<bool> values)
	{
		Value = values.ToArray();
	}

	public void AsStringList(IReadOnlyCollection<string> values)
	{
		Value = values.ToArray();
	}

	public void AsCharList(IReadOnlyCollection<char> values)
	{
		Value = values.ToArray();
	}

	public void AsQuaternionList(IReadOnlyCollection<Quaternion> values)
	{
		Value = values.ToArray();
	}

	public void AsVector3List(IReadOnlyCollection<Vector3> values)
	{
		Value = values.ToArray();
	}

	public void AsVector3IntList(IReadOnlyCollection<Vector3Int> values)
	{
		Value = values.ToArray();
	}

	public void AsVector2List(IReadOnlyCollection<Vector2> values)
	{
		Value = values.ToArray();
	}

	public void AsVector2IntList(IReadOnlyCollection<Vector2Int> values)
	{
		Value = values.ToArray();
	}

	public void AsColorList(IReadOnlyCollection<Color> values)
	{
		Value = values.ToArray();
	}

	public void AsGuidList(IReadOnlyCollection<Guid> values)
	{
		Value = values.ToArray();
	}

	public void AsList<T>(IReadOnlyCollection<T> values, IValueSerializer<T> serializer)
	{
		Value = SaveConversions.ConvertList(values, serializer);
	}

	public IObjectSaver AsObject()
	{
		SerializedObject serializedObject = (SerializedObject)(Value = new SerializedObject());
		return new ObjectSaver(serializedObject);
	}
}
