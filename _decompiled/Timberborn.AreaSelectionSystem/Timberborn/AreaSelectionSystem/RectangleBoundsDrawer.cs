using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

public class RectangleBoundsDrawer
{
	private readonly MeshDrawer _blockBottomMeshDrawer;

	private readonly NeighboredValues4<MeshDrawer> _blockSideMeshDrawers = new NeighboredValues4<MeshDrawer>();

	public RectangleBoundsDrawer(MeshDrawer blockSideMeshDrawer0010, MeshDrawer blockSideMeshDrawer0011, MeshDrawer blockSideMeshDrawer0111, MeshDrawer blockSideMeshDrawer1010, MeshDrawer blockSideMeshDrawer1111, MeshDrawer blockBottomMeshDrawer)
	{
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer0010, down: false, left: false, up: true, right: false);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer0011, down: false, left: false, up: true, right: true);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer0111, down: false, left: true, up: true, right: true);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer1010, down: true, left: false, up: true, right: false);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer1111, down: true, left: true, up: true, right: true);
		_blockBottomMeshDrawer = blockBottomMeshDrawer;
	}

	public void DrawOnLevel(Vector2Int start, Vector2Int end, int level)
	{
		(Vector2Int min, Vector2Int max) tuple = Vectors.MinMax(start, end);
		Vector2Int item = tuple.min;
		Vector2Int item2 = tuple.max;
		for (int i = item.x; i <= item2.x; i++)
		{
			for (int j = item.y; j <= item2.y; j++)
			{
				Vector3Int block = ProjectOnLevel(new Vector2Int(i, j), level);
				DrawBottom(block);
				DrawSides(block, item, item2, level);
			}
		}
	}

	private void DrawBottom(Vector3Int block)
	{
		_blockBottomMeshDrawer.DrawAtCoordinates(block, 0.02f);
	}

	private void DrawSides(Vector3Int block, Vector2Int min, Vector2Int max, int minLevel)
	{
		bool flag = VisibleSide(block, Vector2Int.down, min, max, minLevel);
		bool flag2 = VisibleSide(block, Vector2Int.left, min, max, minLevel);
		bool flag3 = VisibleSide(block, Vector2Int.up, min, max, minLevel);
		bool flag4 = VisibleSide(block, Vector2Int.right, min, max, minLevel);
		if (flag | flag2 | flag3 | flag4)
		{
			var (meshDrawer2, orientation2) = _blockSideMeshDrawers.GetMatch(flag, flag2, flag3, flag4);
			Quaternion rotation = Quaternion.AngleAxis(orientation2.ToAngle(), Vector3.up);
			meshDrawer2.DrawAtCoordinates(block, 0.02f, rotation);
		}
	}

	private static bool VisibleSide(Vector3Int block, Vector2Int neighborOffset, Vector2Int min, Vector2Int max, int minLevel)
	{
		Vector2Int vector2Int = block.XY() + neighborOffset;
		Vector3Int vector3Int = ProjectOnLevel(vector2Int, minLevel);
		if (InBounds(vector2Int, min, max))
		{
			return block.z != vector3Int.z;
		}
		return true;
	}

	private static Vector3Int ProjectOnLevel(Vector2Int block, int level)
	{
		return new Vector3Int(block.x, block.y, level);
	}

	private static bool InBounds(Vector2Int coordinates, Vector2Int min, Vector2Int max)
	{
		if (coordinates.x >= min.x && coordinates.x <= max.x && coordinates.y >= min.y)
		{
			return coordinates.y <= max.y;
		}
		return false;
	}
}
