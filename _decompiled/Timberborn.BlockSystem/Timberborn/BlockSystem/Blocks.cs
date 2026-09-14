using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.Common;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class Blocks
{
	private readonly ImmutableArray<Block> _all;

	public Vector3Int Size { get; }

	private Blocks(Vector3Int size, ImmutableArray<Block> all)
	{
		Size = size;
		_all = all;
	}

	public static Blocks From(BlockObjectSpec blockObjectSpec)
	{
		Vector3Int size = blockObjectSpec.Size;
		ImmutableArray<Block> all = (from coordinates in GetAllCoordinates(size)
			select Block.From(coordinates, blockObjectSpec.BlockSpecFromCoordinates(coordinates))).ToImmutableArray();
		return new Blocks(size, all);
	}

	public IEnumerable<Vector3Int> GetAllCoordinates()
	{
		return GetAllCoordinates(Size);
	}

	public IEnumerable<Vector3Int> GetOccupiedCoordinates()
	{
		return from block in GetOccupiedBlocks()
			select block.Coordinates;
	}

	public ImmutableArray<Block> GetAllBlocks()
	{
		return _all;
	}

	public IEnumerable<Block> GetOccupiedBlocks()
	{
		return _all.Where((Block block) => block.IsOccupied);
	}

	public void PositionBlocks(IList<Block> positionedBlocks, Placement placement)
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			Block block2 = PositionBlock(block, placement);
			positionedBlocks.Add(block2);
			GetBottomBlocks(block2, positionedBlocks);
		}
	}

	public Vector2Int Transform(Vector2Int coordinates, Placement placement)
	{
		return placement.Orientation.Transform(placement.FlipMode.Transform(coordinates, Size.x)) + placement.Coordinates.XY();
	}

	public Vector3Int Transform(Vector3Int coordinates, Placement placement)
	{
		return placement.Orientation.Transform(placement.FlipMode.Transform(coordinates, Size.x)) + placement.Coordinates;
	}

	public Vector3 Transform(Vector3 coordinates, Placement placement)
	{
		return placement.Orientation.Transform(placement.FlipMode.Transform(coordinates, Size.x)) + Pivot(placement.Coordinates, placement.Orientation);
	}

	public Vector3 Pivot(Vector3Int coordinates, Orientation orientation)
	{
		return coordinates + orientation.ToPivotOffset();
	}

	private static IEnumerable<Vector3Int> GetAllCoordinates(Vector3Int size)
	{
		int x = 0;
		while (x < size.x)
		{
			int num;
			for (int y = 0; y < size.y; y = num)
			{
				for (int z = 0; z < size.z; z = num)
				{
					yield return new Vector3Int(x, y, z);
					num = z + 1;
				}
				num = y + 1;
			}
			num = x + 1;
			x = num;
		}
	}

	private Block PositionBlock(Block block, Placement placement)
	{
		return Block.From(Transform(block.Coordinates, placement), block);
	}

	private static void GetBottomBlocks(Block positionedBlock, ICollection<Block> bottomBlocks)
	{
		if (positionedBlock.OccupyAllBelow)
		{
			int num = -1;
			int num2 = positionedBlock.Coordinates.z - 1;
			while (num2 >= 0)
			{
				Vector3Int coordinates = positionedBlock.Coordinates + new Vector3Int(0, 0, num);
				bottomBlocks.Add(Block.FullFrom(coordinates));
				num2--;
				num--;
			}
		}
	}
}
