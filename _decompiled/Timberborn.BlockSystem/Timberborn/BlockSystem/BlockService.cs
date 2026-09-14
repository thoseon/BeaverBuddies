using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

internal class BlockService : IBlockService, ILoadableSingleton
{
	private readonly MapSize _mapSize;

	private readonly EventBus _eventBus;

	private Array3D<WorldBlock> _blocks;

	public Vector3Int Size { get; private set; }

	public BlockService(MapSize mapSize, EventBus eventBus)
	{
		_mapSize = mapSize;
		_eventBus = eventBus;
	}

	public void Load()
	{
		InitializeArrays();
	}

	public bool AnyObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).HasAnyObject;
	}

	public bool AnyTopObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Top;
	}

	public bool BlockNeedsGroundBelow(Vector3Int coordinates)
	{
		foreach (BlockObject blockObject in _blocks.GetCopyAtOrDefault(coordinates).BlockObjects)
		{
			MatterBelow matterBelow = blockObject.PositionedBlocks.GetBlock(coordinates).MatterBelow;
			if ((uint)matterBelow <= 1u)
			{
				return true;
			}
		}
		return false;
	}

	public bool AnyNonOverridableObjectBelow(Vector3Int coordinates)
	{
		int num = -1;
		int num2 = coordinates.z - 1;
		while (num2 >= 0)
		{
			Vector3Int coordinates2 = coordinates + new Vector3Int(0, 0, num);
			if (AnyNonOverridableObjectsAt(coordinates2, BlockOccupations.All))
			{
				return true;
			}
			num2--;
			num--;
		}
		return false;
	}

	public ReadOnlyList<BlockObject> GetObjectsAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).BlockObjects;
	}

	public IEnumerable<BlockObject> GetStackedObjectsAt(Vector3Int coordinates)
	{
		return GetStackObjectsAt(coordinates, _blocks.GetCopyAtOrDefault(coordinates));
	}

	public IEnumerable<BlockObject> GetStackedObjectsWithUndergroundAt(Vector3Int coordinates)
	{
		WorldBlock block = _blocks.GetCopyAtOrDefault(coordinates);
		foreach (BlockObject item in GetStackObjectsAt(coordinates, block))
		{
			yield return item;
		}
		if (NeedsSolidMatterBelow(coordinates, block.Underground))
		{
			yield return block.Underground;
		}
	}

	public IEnumerable<T> GetObjectsWithComponentAt<T>(Vector3Int coordinates)
	{
		foreach (BlockObject item in GetObjectsAt(coordinates))
		{
			if (item.TryGetComponent<T>(out var component))
			{
				yield return component;
			}
		}
	}

	public T GetFirstObjectWithComponentAt<T>(Vector3Int coordinates)
	{
		foreach (BlockObject item in GetObjectsAt(coordinates))
		{
			if (item.TryGetComponent<T>(out var component))
			{
				return component;
			}
		}
		return default(T);
	}

	public void GetIntersectingObjectsAt(Vector3Int coordinates, BlockOccupations occupations, List<BlockObject> result)
	{
		_blocks.GetCopyAtOrDefault(coordinates).GetIntersectingObjects(occupations, result);
	}

	public bool AnyNonOverridableObjectsAt(Vector3Int coordinates, BlockOccupations occupations)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).NonOverridableBlockOccupations.Intersects(occupations);
	}

	public BlockObject GetBottomObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Bottom;
	}

	public BlockObject GetUndergroundObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Underground;
	}

	public T GetBottomObjectComponentAt<T>(Vector3Int coordinates)
	{
		return GetBottomObjectAt(coordinates).GetComponentOfNullable<T>();
	}

	public T GetPathObjectComponentAt<T>(Vector3Int coordinates)
	{
		return GetPathObjectAt(coordinates).GetComponentOfNullable<T>();
	}

	public T GetMiddleObjectComponentAt<T>(Vector3Int coordinates)
	{
		return GetMiddleObjectAt(coordinates).GetComponentOfNullable<T>();
	}

	public T GetTopObjectComponentAt<T>(Vector3Int coordinates)
	{
		return GetTopObjectAt(coordinates).GetComponentOfNullable<T>();
	}

	public BlockObject GetPathObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Path;
	}

	public Directions2D GetEntrancesAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Entrances;
	}

	public bool Contains(Vector3Int coordinates)
	{
		return _blocks.Contains(coordinates);
	}

	public bool Contains(Vector2Int coordinates)
	{
		return _blocks.Contains(coordinates);
	}

	public void SetObject(BlockObject blockObject)
	{
		foreach (Block occupiedAndUndergroundBlock in blockObject.PositionedBlocks.GetOccupiedAndUndergroundBlocks())
		{
			if (Contains(occupiedAndUndergroundBlock.Coordinates))
			{
				_blocks.GetRefAt(occupiedAndUndergroundBlock.Coordinates).SetBlockObject(blockObject, occupiedAndUndergroundBlock);
			}
		}
		if (blockObject.HasEntrance)
		{
			PositionedEntrance positionedEntrance = blockObject.PositionedEntrance;
			Vector3Int coordinates = positionedEntrance.Coordinates;
			if (_blocks.Contains(coordinates))
			{
				_blocks.GetRefAt(positionedEntrance.Coordinates).AddEntrance(positionedEntrance.Direction2D);
			}
		}
		_eventBus.Post(new BlockObjectSetEvent(blockObject));
	}

	public void UnsetObject(BlockObject blockObject)
	{
		foreach (Block occupiedAndUndergroundBlock in blockObject.PositionedBlocks.GetOccupiedAndUndergroundBlocks())
		{
			if (Contains(occupiedAndUndergroundBlock.Coordinates))
			{
				_blocks.GetRefAt(occupiedAndUndergroundBlock.Coordinates).UnsetBlockObject(blockObject, occupiedAndUndergroundBlock);
			}
		}
		if (blockObject.HasEntrance)
		{
			PositionedEntrance positionedEntrance = blockObject.PositionedEntrance;
			Vector3Int coordinates = positionedEntrance.Coordinates;
			if (_blocks.Contains(coordinates))
			{
				_blocks.GetRefAt(positionedEntrance.Coordinates).DeleteEntrance(positionedEntrance.Direction2D);
			}
		}
		_eventBus.Post(new BlockObjectUnsetEvent(blockObject));
	}

	private static IEnumerable<BlockObject> GetStackObjectsAt(Vector3Int coordinates, WorldBlock block)
	{
		if (NeedsSolidMatterBelow(coordinates, block.Floor))
		{
			yield return block.Floor;
		}
		if (NeedsSolidMatterBelow(coordinates, block.Path))
		{
			yield return block.Path;
		}
		if (NeedsSolidMatterBelow(coordinates, block.Bottom))
		{
			yield return block.Bottom;
		}
		if (NeedsSolidMatterBelow(coordinates, block.Corners))
		{
			yield return block.Corners;
		}
	}

	private BlockObject GetMiddleObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Middle;
	}

	private BlockObject GetTopObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Top;
	}

	private void InitializeArrays()
	{
		Size = _mapSize.TotalSize;
		_blocks = new Array3D<WorldBlock>(Size, () => default(WorldBlock));
	}

	private static bool NeedsSolidMatterBelow(Vector3Int coordinates, BlockObject blockObject)
	{
		if ((bool)blockObject)
		{
			return blockObject.PositionedBlocks.GetBlock(coordinates).MatterBelow.IsSolidMatter();
		}
		return false;
	}
}
