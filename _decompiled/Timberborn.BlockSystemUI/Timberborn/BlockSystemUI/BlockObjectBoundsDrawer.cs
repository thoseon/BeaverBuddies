using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.BlockSystemUI;

public class BlockObjectBoundsDrawer
{
	private readonly NeighboredValues4<MeshDrawer> _blockSideMeshDrawers = new NeighboredValues4<MeshDrawer>();

	public BlockObjectBoundsDrawer(MeshDrawer blockSideMeshDrawer0010, MeshDrawer blockSideMeshDrawer0011, MeshDrawer blockSideMeshDrawer0111, MeshDrawer blockSideMeshDrawer1010, MeshDrawer blockSideMeshDrawer1111)
	{
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer0010, down: false, left: false, up: true, right: false);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer0011, down: false, left: false, up: true, right: true);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer0111, down: false, left: true, up: true, right: true);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer1010, down: true, left: false, up: true, right: false);
		_blockSideMeshDrawers.AddVariants(blockSideMeshDrawer1111, down: true, left: true, up: true, right: true);
	}

	public void DrawBounds(BlockObject blockObject)
	{
		BlockObjectModelController component = blockObject.GetComponent<BlockObjectModelController>();
		if (component == null || component.IsAnyModelShown)
		{
			int bottomLevel = blockObject.CoordinatesAtBaseZ.z;
			IEnumerable<Vector3Int> source = from coordinates in blockObject.PositionedBlocks.GetOccupiedCoordinates()
				where coordinates.z == bottomLevel
				select coordinates;
			DrawBounds(source.ToList());
		}
	}

	private void DrawBounds(ICollection<Vector3Int> occupiedCoordinates)
	{
		foreach (Vector3Int occupiedCoordinate in occupiedCoordinates)
		{
			bool flag = VisibleSide(occupiedCoordinate, Vector3Int.down, occupiedCoordinates);
			bool flag2 = VisibleSide(occupiedCoordinate, Vector3Int.left, occupiedCoordinates);
			bool flag3 = VisibleSide(occupiedCoordinate, Vector3Int.up, occupiedCoordinates);
			bool flag4 = VisibleSide(occupiedCoordinate, Vector3Int.right, occupiedCoordinates);
			if (flag | flag2 | flag3 | flag4)
			{
				var (meshDrawer2, orientation2) = _blockSideMeshDrawers.GetMatch(flag, flag2, flag3, flag4);
				Quaternion rotation = Quaternion.AngleAxis(orientation2.ToAngle(), Vector3.up);
				meshDrawer2.DrawAtCoordinates(occupiedCoordinate, 0.02f, rotation);
			}
		}
	}

	private static bool VisibleSide(Vector3Int coordinate, Vector3Int delta, ICollection<Vector3Int> coordinates)
	{
		return !coordinates.Contains(coordinate + delta);
	}
}
