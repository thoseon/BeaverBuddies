using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Persistence;

public interface IValueLoader
{
	int AsInt();

	float AsFloat();

	bool AsBool();

	string AsString();

	char AsChar();

	Quaternion AsQuaternion();

	Vector3 AsVector3();

	Vector3Int AsVector3Int();

	Vector2 AsVector2();

	Vector2Int AsVector2Int();

	Color AsColor();

	Guid AsGuid();

	T As<T>(IValueSerializer<T> serializer);

	bool AsObsoletable<T>(IValueSerializer<T> serializer, out T value);

	List<int> AsIntList();

	List<float> AsFloatList();

	List<bool> AsBoolList();

	List<string> AsStringList();

	List<char> AsCharList();

	List<Quaternion> AsQuaternionList();

	List<Vector3> AsVector3List();

	List<Vector3Int> AsVector3IntList();

	List<Vector2> AsVector2List();

	List<Vector2Int> AsVector2IntList();

	List<Color> AsColorList();

	List<Guid> AsGuidList();

	List<T> AsList<T>(IValueSerializer<T> serializer);

	IObjectLoader AsObject();

	bool IsObject();

	bool IsList();
}
