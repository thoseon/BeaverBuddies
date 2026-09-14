using UnityEngine;

namespace Timberborn.Coordinates;

public static class Deltas
{
	public static readonly Vector3Int[] Corners4Vector3Int = new Vector3Int[4]
	{
		new Vector3Int(-1, -1, 0),
		new Vector3Int(-1, 1, 0),
		new Vector3Int(1, 1, 0),
		new Vector3Int(1, -1, 0)
	};

	public static readonly Vector3Int[] Neighbors4Vector3Int = new Vector3Int[4]
	{
		new Vector3Int(-1, 0, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(0, -1, 0)
	};

	public static readonly Vector2Int[] Neighbors4Vector2Int = new Vector2Int[4]
	{
		new Vector2Int(-1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(1, 0),
		new Vector2Int(0, -1)
	};

	public static readonly Vector3Int[] Neighbors8AndSelfVector3Int = new Vector3Int[9]
	{
		new Vector3Int(-1, 0, 0),
		new Vector3Int(-1, 1, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(1, 1, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(1, -1, 0),
		new Vector3Int(0, -1, 0),
		new Vector3Int(-1, -1, 0),
		new Vector3Int(0, 0, 0)
	};

	public static readonly Vector2Int[] Neighbors8AndSelfVector2Int = new Vector2Int[9]
	{
		new Vector2Int(-1, 0),
		new Vector2Int(-1, 1),
		new Vector2Int(0, 1),
		new Vector2Int(1, 1),
		new Vector2Int(1, 0),
		new Vector2Int(1, -1),
		new Vector2Int(0, -1),
		new Vector2Int(-1, -1),
		new Vector2Int(0, 0)
	};

	public static readonly Vector3Int[] Neighbors8Vector3IntOrdered = new Vector3Int[8]
	{
		new Vector3Int(-1, 0, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(0, -1, 0),
		new Vector3Int(-1, -1, 0),
		new Vector3Int(-1, 1, 0),
		new Vector3Int(1, 1, 0),
		new Vector3Int(1, -1, 0)
	};

	public static readonly Vector3Int[] Neighbors6Vector3Int = new Vector3Int[6]
	{
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(0, -1, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, 0, -1),
		new Vector3Int(0, 0, 1)
	};

	public static readonly Vector3Int[] Neighbors26Vector3Int = new Vector3Int[26]
	{
		new Vector3Int(-1, -1, -1),
		new Vector3Int(0, -1, -1),
		new Vector3Int(1, -1, -1),
		new Vector3Int(-1, 0, -1),
		new Vector3Int(0, 0, -1),
		new Vector3Int(1, 0, -1),
		new Vector3Int(-1, 1, -1),
		new Vector3Int(0, 1, -1),
		new Vector3Int(1, 1, -1),
		new Vector3Int(-1, -1, 0),
		new Vector3Int(0, -1, 0),
		new Vector3Int(1, -1, 0),
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(-1, 1, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(1, 1, 0),
		new Vector3Int(-1, -1, 1),
		new Vector3Int(0, -1, 1),
		new Vector3Int(1, -1, 1),
		new Vector3Int(-1, 0, 1),
		new Vector3Int(0, 0, 1),
		new Vector3Int(1, 0, 1),
		new Vector3Int(-1, 1, 1),
		new Vector3Int(0, 1, 1),
		new Vector3Int(1, 1, 1)
	};
}
