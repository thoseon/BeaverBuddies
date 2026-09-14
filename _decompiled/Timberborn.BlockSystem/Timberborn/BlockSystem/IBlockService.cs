using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BlockSystem;

public interface IBlockService
{
	Vector3Int Size { get; }

	bool AnyObjectAt(Vector3Int coordinates);

	bool AnyTopObjectAt(Vector3Int coordinates);

	bool BlockNeedsGroundBelow(Vector3Int coordinates);

	bool AnyNonOverridableObjectBelow(Vector3Int coordinates);

	ReadOnlyList<BlockObject> GetObjectsAt(Vector3Int coordinates);

	IEnumerable<BlockObject> GetStackedObjectsAt(Vector3Int coordinates);

	IEnumerable<BlockObject> GetStackedObjectsWithUndergroundAt(Vector3Int coordinates);

	IEnumerable<T> GetObjectsWithComponentAt<T>(Vector3Int coordinates);

	T GetFirstObjectWithComponentAt<T>(Vector3Int coordinates);

	void GetIntersectingObjectsAt(Vector3Int coordinates, BlockOccupations occupations, List<BlockObject> result);

	bool AnyNonOverridableObjectsAt(Vector3Int coordinates, BlockOccupations occupations);

	BlockObject GetBottomObjectAt(Vector3Int coordinates);

	BlockObject GetUndergroundObjectAt(Vector3Int coordinates);

	T GetBottomObjectComponentAt<T>(Vector3Int coordinates);

	T GetPathObjectComponentAt<T>(Vector3Int coordinates);

	T GetMiddleObjectComponentAt<T>(Vector3Int coordinates);

	T GetTopObjectComponentAt<T>(Vector3Int coordinates);

	BlockObject GetPathObjectAt(Vector3Int coordinates);

	Directions2D GetEntrancesAt(Vector3Int coordinates);

	bool Contains(Vector3Int coordinates);

	bool Contains(Vector2Int coordinates);
}
