using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Persistence;

public interface IValueSaver
{
	void AsInt(int value);

	void AsFloat(float value);

	void AsBool(bool value);

	void AsString(string value);

	void AsChar(char value);

	void AsQuaternion(Quaternion value);

	void AsVector3(Vector3 value);

	void AsVector3Int(Vector3Int value);

	void AsVector2(Vector2 value);

	void AsVector2Int(Vector2Int value);

	void AsColor(Color value);

	void AsGuid(Guid value);

	void As<T>(T value, IValueSerializer<T> serializer);

	void AsIntList(IReadOnlyCollection<int> values);

	void AsFloatList(IReadOnlyCollection<float> values);

	void AsBoolList(IReadOnlyCollection<bool> values);

	void AsStringList(IReadOnlyCollection<string> values);

	void AsCharList(IReadOnlyCollection<char> values);

	void AsQuaternionList(IReadOnlyCollection<Quaternion> values);

	void AsVector3List(IReadOnlyCollection<Vector3> values);

	void AsVector3IntList(IReadOnlyCollection<Vector3Int> values);

	void AsVector2List(IReadOnlyCollection<Vector2> values);

	void AsVector2IntList(IReadOnlyCollection<Vector2Int> values);

	void AsColorList(IReadOnlyCollection<Color> values);

	void AsGuidList(IReadOnlyCollection<Guid> values);

	void AsList<T>(IReadOnlyCollection<T> values, IValueSerializer<T> serializer);

	IObjectSaver AsObject();
}
