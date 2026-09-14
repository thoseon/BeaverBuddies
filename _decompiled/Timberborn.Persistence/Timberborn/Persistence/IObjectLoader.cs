using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Persistence;

public interface IObjectLoader
{
	int Get(PropertyKey<int> key);

	float Get(PropertyKey<float> key);

	bool Get(PropertyKey<bool> key);

	string Get(PropertyKey<string> key);

	char Get(PropertyKey<char> key);

	Quaternion Get(PropertyKey<Quaternion> key);

	Vector3 Get(PropertyKey<Vector3> key);

	Vector3Int Get(PropertyKey<Vector3Int> key);

	Vector2 Get(PropertyKey<Vector2> key);

	Vector2Int Get(PropertyKey<Vector2Int> key);

	Color Get(PropertyKey<Color> key);

	Guid Get(PropertyKey<Guid> key);

	T Get<T>(PropertyKey<T> key) where T : Enum;

	T Get<T>(PropertyKey<T> key, IValueSerializer<T> serializer);

	bool GetObsoletable<T>(PropertyKey<T> key, IValueSerializer<T> serializer, out T value);

	List<int> Get(ListKey<int> key);

	List<float> Get(ListKey<float> key);

	List<bool> Get(ListKey<bool> key);

	List<string> Get(ListKey<string> key);

	List<char> Get(ListKey<char> key);

	List<Quaternion> Get(ListKey<Quaternion> key);

	List<Vector3> Get(ListKey<Vector3> key);

	List<Vector3Int> Get(ListKey<Vector3Int> key);

	List<Vector2> Get(ListKey<Vector2> key);

	List<Vector2Int> Get(ListKey<Vector2Int> key);

	List<Color> Get(ListKey<Color> key);

	List<Guid> Get(ListKey<Guid> key);

	List<T> Get<T>(ListKey<T> key) where T : Enum;

	List<T> Get<T>(ListKey<T> key, IValueSerializer<T> serializer);

	bool Has<T>(PropertyKey<T> key);

	bool Has<T>(ListKey<T> key);
}
