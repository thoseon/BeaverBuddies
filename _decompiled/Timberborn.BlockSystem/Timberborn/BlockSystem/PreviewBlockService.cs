using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class PreviewBlockService : ILoadableSingleton
{
	private readonly IBlockService _blockService;

	private Array3D<WorldBlock> _blocks;

	public PreviewBlockService(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void Load()
	{
		_blocks = new Array3D<WorldBlock>(_blockService.Size, () => default(WorldBlock));
	}

	public void SetPreview(BlockObject blockObject)
	{
		SetOrUnsetPreview(blockObject, set: true);
	}

	public void UnsetPreview(BlockObject blockObject)
	{
		SetOrUnsetPreview(blockObject, set: false);
	}

	public BlockObject GetBottomPreviewAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Bottom;
	}

	public IEnumerable<BlockObject> GetPreviewsAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).BlockObjects;
	}

	public BlockObject GetPathObjectAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Path;
	}

	public T GetBottomObjectComponentAt<T>(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Bottom.GetComponentOfNullable<T>();
	}

	public T GetPathObjectComponentAt<T>(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Path.GetComponentOfNullable<T>();
	}

	public IEnumerable<T> GetObjectsWithComponentAt<T>(Vector3Int coordinates) where T : BaseComponent
	{
		foreach (BlockObject blockObject in _blocks.GetCopyAtOrDefault(coordinates).BlockObjects)
		{
			if (blockObject.TryGetComponent<T>(out var component))
			{
				yield return component;
			}
		}
	}

	public T GetFirstObjectWithComponentAt<T>(Vector3Int coordinates)
	{
		foreach (BlockObject blockObject in _blocks.GetCopyAtOrDefault(coordinates).BlockObjects)
		{
			if (blockObject.TryGetComponent<T>(out var component))
			{
				return component;
			}
		}
		return default(T);
	}

	public Directions2D GetEntrancesAt(Vector3Int coordinates)
	{
		return _blocks.GetCopyAtOrDefault(coordinates).Entrances;
	}

	public bool IsUnfinishedGroundBlockAt(Vector3Int coordinates)
	{
		ReadOnlyList<BlockObject> blockObjects = _blocks.GetCopyAtOrDefault(coordinates).BlockObjects;
		for (int i = 0; i < blockObjects.Count; i++)
		{
			if (blockObjects[i].PositionedBlocks.GetBlock(coordinates).Stackable.IsUnfinishedGround())
			{
				return true;
			}
		}
		return false;
	}

	private void SetOrUnsetPreview(BlockObject blockObject, bool set)
	{
		SetOrUnsetBlocks(blockObject, set);
		SetOrUnsetEntrance(blockObject, set);
	}

	private void SetOrUnsetBlocks(BlockObject blockObject, bool set)
	{
		foreach (Block allBlock in blockObject.PositionedBlocks.GetAllBlocks())
		{
			Vector3Int coordinates = allBlock.Coordinates;
			if (_blocks.Contains(coordinates))
			{
				ref WorldBlock refAt = ref _blocks.GetRefAt(coordinates);
				if (set)
				{
					refAt.SetBlockObject(blockObject, allBlock);
				}
				else
				{
					refAt.UnsetBlockObject(blockObject, allBlock);
				}
			}
		}
	}

	private void SetOrUnsetEntrance(BlockObject blockObject, bool set)
	{
		if (!blockObject.HasEntrance)
		{
			return;
		}
		PositionedEntrance positionedEntrance = blockObject.PositionedEntrance;
		Vector3Int coordinates = positionedEntrance.Coordinates;
		if (_blocks.Contains(coordinates))
		{
			ref WorldBlock refAt = ref _blocks.GetRefAt(coordinates);
			if (set)
			{
				refAt.AddEntrance(positionedEntrance.Direction2D);
			}
			else
			{
				refAt.DeleteEntrance(positionedEntrance.Direction2D);
			}
		}
	}
}
