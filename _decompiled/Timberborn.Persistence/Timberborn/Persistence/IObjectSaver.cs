using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Persistence;

public interface IObjectSaver
{
	void Set(PropertyKey<int> key, int value);

	void Set(PropertyKey<float> key, float value);

	void Set(PropertyKey<bool> key, bool value);

	void Set(PropertyKey<string> key, string value);

	void Set(PropertyKey<char> key, char value);

	void Set(PropertyKey<Quaternion> key, Quaternion value);

	void Set(PropertyKey<Vector3> key, Vector3 value);

	void Set(PropertyKey<Vector3Int> key, Vector3Int value);

	void Set(PropertyKey<Vector2> key, Vector2 value);

	void Set(PropertyKey<Vector2Int> key, Vector2Int value);

	void Set(PropertyKey<Color> key, Color value);

	void Set(PropertyKey<Guid> key, Guid value);

	void Set<T>(PropertyKey<T> key, T value) where T : Enum;

	void Set<T>(PropertyKey<T> key, T value, IValueSerializer<T> serializer);

	void Set(ListKey<int> key, IReadOnlyCollection<int> values);

	void Set(ListKey<float> key, IReadOnlyCollection<float> values);

	void Set(ListKey<bool> key, IReadOnlyCollection<bool> values);

	void Set(ListKey<string> key, IReadOnlyCollection<string> values);

	void Set(ListKey<char> key, IReadOnlyCollection<char> values);

	void Set(ListKey<Quaternion> key, IReadOnlyCollection<Quaternion> values);

	void Set(ListKey<Vector3> key, IReadOnlyCollection<Vector3> values);

	void Set(ListKey<Vector3Int> key, IReadOnlyCollection<Vector3Int> values);

	void Set(ListKey<Vector2> key, IReadOnlyCollection<Vector2> values);

	void Set(ListKey<Vector2Int> key, IReadOnlyCollection<Vector2Int> values);

	void Set(ListKey<Color> key, IReadOnlyCollection<Color> values);

	void Set(ListKey<Guid> key, IReadOnlyCollection<Guid> values);

	void Set<T>(ListKey<T> key, IReadOnlyCollection<T> values) where T : Enum;

	void Set<T>(ListKey<T> key, IReadOnlyCollection<T> values, IValueSerializer<T> serializer);
}
