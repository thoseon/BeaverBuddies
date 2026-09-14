using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class PositionedBlocks
{
	private readonly ImmutableArray<Block> _all;

	private PositionedBlocks(ImmutableArray<Block> all)
	{
		_all = all;
	}

	public static PositionedBlocks From(Blocks blocks, Placement placement)
	{
		List<Block> list = new List<Block>();
		blocks.PositionBlocks(list, placement);
		return new PositionedBlocks(list.ToImmutableArray());
	}

	public ImmutableArray<Block> GetAllBlocks()
	{
		return _all;
	}

	public IEnumerable<Block> GetOccupiedBlocks()
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.IsOccupied)
			{
				yield return block;
			}
		}
	}

	public IEnumerable<Block> GetOccupiedStackableBlocks()
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.IsOccupied && block.Stackable.IsStackable())
			{
				yield return block;
			}
		}
	}

	public IEnumerable<Block> GetOccupiedAndUndergroundBlocks()
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.IsOccupied || block.Underground)
			{
				yield return block;
			}
		}
	}

	public IEnumerable<Block> GetFoundationBlocks()
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.IsFoundationBlock)
			{
				yield return block;
			}
		}
	}

	public IEnumerable<Vector3Int> GetFoundationCoordinates()
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.IsFoundationBlock)
			{
				yield return block.Coordinates;
			}
		}
	}

	public IEnumerable<Vector3Int> GetAllCoordinates()
	{
		for (int i = 0; i < _all.Length; i++)
		{
			yield return _all[i].Coordinates;
		}
	}

	public IEnumerable<Vector3Int> GetOccupiedCoordinates()
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.IsOccupied)
			{
				yield return block.Coordinates;
			}
		}
	}

	public IEnumerable<Vector3Int> GetOccupiedCoordinatesIntersecting(BlockOccupations occupation)
	{
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.Occupation.Intersects(occupation))
			{
				yield return block.Coordinates;
			}
		}
	}

	public bool TryGetBlock(Vector3Int coordinates, out Block result)
	{
		result = default(Block);
		for (int i = 0; i < _all.Length; i++)
		{
			Block block = _all[i];
			if (block.Coordinates == coordinates)
			{
				result = block;
				return true;
			}
		}
		return false;
	}

	public Block GetBlock(Vector3Int coordinates)
	{
		if (TryGetBlock(coordinates, out var result))
		{
			return result;
		}
		throw new NullReferenceException(string.Format("No {0} found at {1}", "Block", coordinates));
	}

	public bool HasIntersectingBlock(Block block)
	{
		for (int i = 0; i < _all.Length; i++)
		{
			if (_all[i].IsIntersecting(block))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasBlockAt(Vector3Int coordinates)
	{
		foreach (Block item in _all)
		{
			if (item.Coordinates == coordinates)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasStackableBlockAt(Vector3Int coordinates)
	{
		foreach (Block item in _all)
		{
			if (item.Coordinates == coordinates && item.Stackable.IsStackable())
			{
				return true;
			}
		}
		return false;
	}
}
