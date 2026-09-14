using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.LevelVisibilitySystem;
using UnityEngine;

namespace Timberborn.BlockObjectPickingSystem;

public class BlockObjectPicker
{
	private readonly StackedBlockObjectPicker _stackedBlockObjectPicker;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly AreaIterator _areaIterator;

	private readonly IBlockService _blockService;

	public BlockObjectPicker(StackedBlockObjectPicker stackedBlockObjectPicker, ILevelVisibilityService levelVisibilityService, AreaIterator areaIterator, IBlockService blockService)
	{
		_stackedBlockObjectPicker = stackedBlockObjectPicker;
		_levelVisibilityService = levelVisibilityService;
		_areaIterator = areaIterator;
		_blockService = blockService;
	}

	public IEnumerable<BlockObject> PickBlockObjects(SelectionStart selectionStart, Vector3Int endCoords, BlockObjectPickingMode pickingMode, Func<BlockObject, bool> blockObjectFilter, bool selectingArea)
	{
		Vector3Int coordinates = selectionStart.Coordinates;
		BlockObjectHit? blockObjectHit = selectionStart.GetBlockObjectHit();
		BlockObject blockObject = blockObjectHit?.BlockObject;
		if (blockObject != null)
		{
			if (!selectingArea && !IsStackable(blockObject) && (blockObjectFilter == null || blockObjectFilter(blockObject)) && pickingMode != BlockObjectPickingMode.DownwardStack)
			{
				return Enumerables.One(blockObject);
			}
			if (blockObject.BaseZ > 0)
			{
				coordinates.z = blockObject.CoordinatesAtBaseZ.z;
			}
		}
		BlockObjectPickerFilter filter = (blockObjectHit.HasValue ? BlockObjectPickerFilter.CreateWithConstraints(blockObjectHit.Value, coordinates, _levelVisibilityService.MaxVisibleLevel, blockObjectFilter) : BlockObjectPickerFilter.Create(coordinates.z, blockObjectFilter));
		if (pickingMode == BlockObjectPickingMode.InsideArea)
		{
			return PickBlockObjectsInArea(coordinates, endCoords, filter);
		}
		return PickBlockObjectsInStack(blockObject, coordinates, endCoords, pickingMode, selectingArea, filter);
	}

	private static bool IsStackable(BlockObject blockObject)
	{
		return blockObject.PositionedBlocks.GetAllBlocks().Any((Block block) => block.Stackable.IsStackable());
	}

	private IEnumerable<BlockObject> PickBlockObjectsInArea(Vector3Int startCoords, Vector3Int endCoords, BlockObjectPickerFilter filter)
	{
		IEnumerable<BlockObject> enumerable = (from coords in _areaIterator.GetCuboid(startCoords, endCoords)
			where _blockService.AnyObjectAt(coords)
			select coords).SelectMany((Vector3Int coords) => GetValidObjects(coords, filter)).Distinct();
		foreach (BlockObject item in enumerable)
		{
			yield return item;
		}
	}

	private IEnumerable<BlockObject> PickBlockObjectsInStack(BlockObject startBlockObject, Vector3Int startCoords, Vector3Int endCoords, BlockObjectPickingMode pickingMode, bool selectingArea, BlockObjectPickerFilter filter)
	{
		if (selectingArea)
		{
			return _stackedBlockObjectPicker.GetStackOfBlockObjectsInArea(startCoords, endCoords, pickingMode, filter);
		}
		return _stackedBlockObjectPicker.GetStackOfBlockObjectsFromBlockObject(startBlockObject, pickingMode, filter);
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
}
