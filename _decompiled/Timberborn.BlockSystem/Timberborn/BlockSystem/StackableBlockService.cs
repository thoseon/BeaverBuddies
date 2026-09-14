using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.LevelVisibilitySystem;
using Timberborn.MapStateSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class StackableBlockService
{
	private readonly IBlockService _blockService;

	private readonly ITerrainService _terrainService;

	private readonly MapSize _mapSize;

	private readonly ILevelVisibilityService _levelVisibilityService;

	public StackableBlockService(IBlockService blockService, ITerrainService terrainService, MapSize mapSize, ILevelVisibilityService levelVisibilityService)
	{
		_blockService = blockService;
		_terrainService = terrainService;
		_mapSize = mapSize;
		_levelVisibilityService = levelVisibilityService;
	}

	public bool IsStackableBlockAt(Vector3Int coords, bool finishedOnly = false)
	{
		ReadOnlyList<BlockObject> objectsAt = _blockService.GetObjectsAt(coords);
		for (int i = 0; i < objectsAt.Count; i++)
		{
			BlockObject blockObject = objectsAt[i];
			if ((!finishedOnly || blockObject.IsFinished) && blockObject.PositionedBlocks.GetBlock(coords).Stackable.IsStackable())
			{
				return true;
			}
		}
		return false;
	}

	public bool IsFinishedStackableBlockAt(Vector3Int coords)
	{
		ReadOnlyList<BlockObject> objectsAt = _blockService.GetObjectsAt(coords);
		for (int i = 0; i < objectsAt.Count; i++)
		{
			BlockObject blockObject = objectsAt[i];
			if (blockObject.IsFinished && blockObject.PositionedBlocks.GetBlock(coords).Stackable.IsStackable())
			{
				return true;
			}
		}
		return false;
	}

	public bool IsUnfinishedGroundBlockAt(Vector3Int coords)
	{
		ReadOnlyList<BlockObject> objectsAt = _blockService.GetObjectsAt(coords);
		for (int i = 0; i < objectsAt.Count; i++)
		{
			if (objectsAt[i].PositionedBlocks.GetBlock(coords).Stackable.IsUnfinishedGround())
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerable<Vector3Int> GetGroundOrStackableBlocks(IEnumerable<Vector2Int> coordinates, bool finishedOnly = false)
	{
		foreach (Vector2Int coordinate in coordinates)
		{
			for (int z = 0; z < _mapSize.TotalSize.z; z++)
			{
				Vector3Int vector3Int = coordinate.ToVector3Int(z);
				if ((_terrainService.OnGround(vector3Int) && z <= _levelVisibilityService.MaxVisibleLevel) || IsVisibleStackableAt(vector3Int, finishedOnly))
				{
					yield return vector3Int;
				}
			}
		}
	}

	private bool IsVisibleStackableAt(Vector3Int coords, bool finishedOnly)
	{
		if (_levelVisibilityService.BlockIsVisible(coords))
		{
			return IsStackableBlockAt(new Vector3Int(coords.x, coords.y, coords.z - 1), finishedOnly);
		}
		return false;
	}
}
