using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.BlockObjectPickingSystem;

public class StackedBlockObjectPicker
{
	private readonly AreaIterator _areaIterator;

	private readonly IBlockService _blockService;

	private readonly HashSet<BlockObject> _blockObjects = new HashSet<BlockObject>();

	public StackedBlockObjectPicker(AreaIterator areaIterator, IBlockService blockService)
	{
		_areaIterator = areaIterator;
		_blockService = blockService;
	}

	public IEnumerable<BlockObject> GetStackOfBlockObjectsInArea(Vector3Int start, Vector3Int end, BlockObjectPickingMode pickingMode, BlockObjectPickerFilter selectionFilter)
	{
		_blockObjects.Clear();
		if (pickingMode != BlockObjectPickingMode.DownwardStack && pickingMode != BlockObjectPickingMode.UpwardStack)
		{
			throw new ArgumentException($"Invalid picking mode: {pickingMode}.");
		}
		foreach (BlockObject item in GetBlockObjectsInCuboid(start, end, selectionFilter))
		{
			AddBlockObjectsRecursively(item, pickingMode);
		}
		return _blockObjects.AsReadOnlyEnumerable();
	}

	public IEnumerable<BlockObject> GetStackOfBlockObjectsFromBlockObject(BlockObject startBlockObject, BlockObjectPickingMode pickingMode, BlockObjectPickerFilter selectionFilter)
	{
		_blockObjects.Clear();
		if (startBlockObject != null && selectionFilter.IsValid(startBlockObject))
		{
			AddBlockObjectsRecursively(startBlockObject, pickingMode);
		}
		return _blockObjects.AsReadOnlyEnumerable();
	}

	private void AddBlockObjectsRecursively(BlockObject blockObject, BlockObjectPickingMode pickingMode)
	{
		if (_blockObjects.Add(blockObject))
		{
			AddConnectedBlockObjects(blockObject, pickingMode);
		}
	}

	private IEnumerable<BlockObject> GetBlockObjectsInCuboid(Vector3Int start, Vector3Int end, BlockObjectPickerFilter selectionFilter)
	{
		return (from coords in _areaIterator.GetCuboid(start, end)
			where _blockService.AnyObjectAt(coords)
			select coords).SelectMany((Vector3Int coords) => GetValidObjects(coords, selectionFilter)).Distinct();
	}

	private void AddConnectedBlockObjects(BlockObject blockObject, BlockObjectPickingMode pickingMode)
	{
		foreach (Block item in blockObject.PositionedBlocks.GetAllBlocks().Where(delegate(Block block)
		{
			if (pickingMode != BlockObjectPickingMode.DownwardStack)
			{
				return block.Stackable.IsStackable();
			}
			return block.IsFoundationBlock || block.Stackable.IsUnfinishedGround();
		}))
		{
			AddValidBlockObjectStackedWithBlock(item, pickingMode);
		}
	}

	private IEnumerable<BlockObject> GetValidObjects(Vector3Int coords, BlockObjectPickerFilter selectionFilter)
	{
		foreach (BlockObject item in _blockService.GetObjectsAt(coords))
		{
			if (selectionFilter.IsValid(coords, item))
			{
				yield return item;
			}
		}
	}

	private void AddValidBlockObjectStackedWithBlock(Block block, BlockObjectPickingMode pickingMode)
	{
		int z = ((pickingMode == BlockObjectPickingMode.UpwardStack) ? 1 : (-1));
		Vector3Int coordinates = block.Coordinates + new Vector3Int(0, 0, z);
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			if (ShouldIncludeNearBlock(item.PositionedBlocks.GetBlock(coordinates), pickingMode))
			{
				AddBlockObjectsRecursively(item, pickingMode);
			}
		}
	}

	private static bool ShouldIncludeNearBlock(Block block, BlockObjectPickingMode direction)
	{
		if (direction != BlockObjectPickingMode.UpwardStack)
		{
			return block.Stackable.IsStackable();
		}
		if (!block.IsFoundationBlock)
		{
			return block.Stackable.IsUnfinishedGround();
		}
		return true;
	}
}
