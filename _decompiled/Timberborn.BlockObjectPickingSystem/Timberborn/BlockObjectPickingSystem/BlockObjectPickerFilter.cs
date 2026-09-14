using System;
using System.Linq;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.BlockObjectPickingSystem;

public readonly struct BlockObjectPickerFilter
{
	private readonly int _referenceZ;

	private readonly bool _ignoreTopBlockObjectsOnBaseZ;

	private readonly bool _ignoreBottomBlockObjectsOnBaseZ;

	private readonly BlockOccupations _blockOccupation;

	private readonly Func<BlockObject, bool> _selectionPredicate;

	private BlockObjectPickerFilter(int referenceZ, bool ignoreTopBlockObjectsOnBaseZ, bool ignoreBottomBlockObjectsOnBaseZ, BlockOccupations blockOccupations, Func<BlockObject, bool> selectionPredicate)
	{
		_referenceZ = referenceZ;
		_ignoreTopBlockObjectsOnBaseZ = ignoreTopBlockObjectsOnBaseZ;
		_ignoreBottomBlockObjectsOnBaseZ = ignoreBottomBlockObjectsOnBaseZ;
		_blockOccupation = blockOccupations;
		_selectionPredicate = selectionPredicate;
	}

	public static BlockObjectPickerFilter Create(int referenceZ, Func<BlockObject, bool> selectionPredicate)
	{
		BlockOccupations blockOccupations = (BlockOccupations)int.MaxValue;
		return new BlockObjectPickerFilter(referenceZ, ignoreTopBlockObjectsOnBaseZ: false, ignoreBottomBlockObjectsOnBaseZ: false, blockOccupations, selectionPredicate);
	}

	public static BlockObjectPickerFilter CreateWithConstraints(BlockObjectHit blockObjectHit, Vector3Int startCoords, int maxVisibleLevel, Func<BlockObject, bool> selectionPredicate)
	{
		BlockObject blockObject = blockObjectHit.BlockObject;
		bool ignoreTopBlockObjectsOnBaseZ = startCoords.z == maxVisibleLevel && IsBlockWithBottomOccupation(blockObject, startCoords);
		bool ignoreBottomBlockObjectsOnBaseZ = IsBlockWithTopOccupation(blockObject, startCoords);
		BlockOccupations occupation = blockObjectHit.HitBlock.Occupation;
		return new BlockObjectPickerFilter(startCoords.z, ignoreTopBlockObjectsOnBaseZ, ignoreBottomBlockObjectsOnBaseZ, occupation, selectionPredicate);
	}

	public bool IsValid(BlockObject blockObject)
	{
		if (IsOnValidLevel(blockObject) && blockObject.Blocks.GetAllBlocks().Any(ValidateBlockOccupation))
		{
			return _selectionPredicate?.Invoke(blockObject) ?? true;
		}
		return false;
	}

	public bool IsValid(Vector3Int coords, BlockObject blockObject)
	{
		if (IsOnValidLevel(blockObject) && ValidateBlockOccupation(blockObject.PositionedBlocks.GetBlock(coords)))
		{
			return _selectionPredicate?.Invoke(blockObject) ?? true;
		}
		return false;
	}

	private static bool IsBlockWithBottomOccupation(BlockObject blockObject, Vector3Int coordinates)
	{
		return blockObject.PositionedBlocks.GetBlock(coordinates).Occupation.IsBottomOrFloorOrBoth();
	}

	private static bool IsBlockWithTopOccupation(BlockObject blockObject, Vector3Int coordinates)
	{
		return blockObject.PositionedBlocks.GetBlock(coordinates).Occupation.IsTopOrCornersOrBoth();
	}

	private bool IsOnValidLevel(BlockObject blockObject)
	{
		if (blockObject.CoordinatesAtBaseZ.z == _referenceZ && (!_ignoreTopBlockObjectsOnBaseZ || !HasTopOccupationOnBaseZ(blockObject, _referenceZ)))
		{
			if (_ignoreBottomBlockObjectsOnBaseZ)
			{
				return !HasBottomOccupationOnBaseZ(blockObject, _referenceZ);
			}
			return true;
		}
		return false;
	}

	private static bool HasBottomOccupationOnBaseZ(BlockObject blockObject, int baseZ)
	{
		return (from block in blockObject.PositionedBlocks.GetAllBlocks()
			where block.Coordinates.z == baseZ
			select block).Any((Block block) => block.Occupation.IsBottomOrFloorOrBoth());
	}

	private static bool HasTopOccupationOnBaseZ(BlockObject blockObject, int baseZ)
	{
		return (from block in blockObject.PositionedBlocks.GetAllBlocks()
			where block.Coordinates.z == baseZ
			select block).Any((Block block) => block.Occupation.IsTopOrCornersOrBoth());
	}

	private bool ValidateBlockOccupation(Block block)
	{
		return (block.Occupation & _blockOccupation) != 0;
	}
}
