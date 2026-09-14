using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.TerrainQueryingSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockObjectRange : BaseComponent, IAwakableComponent
{
	private readonly TerrainAreaService _terrainAreaService;

	private readonly StackableBlockService _stackableBlockService;

	private BlockObject _blockObject;

	private BlockObjectCenter _blockObjectCenter;

	public BlockObjectRange(TerrainAreaService terrainAreaService, StackableBlockService stackableBlockService)
	{
		_terrainAreaService = terrainAreaService;
		_stackableBlockService = stackableBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
	}

	public IEnumerable<Vector2Int> GetBlocksInRectangularRadius(int radius)
	{
		Vector2 vector = _blockObjectCenter.GridCenter.XY();
		Vector2Int vector2Int = RotatedSize();
		(int, int) areaBounds = GetAreaBounds(vector.x, vector2Int.x, radius);
		int item = areaBounds.Item1;
		int maxX = areaBounds.Item2;
		areaBounds = GetAreaBounds(vector.y, vector2Int.y, radius);
		int minY = areaBounds.Item1;
		int maxY = areaBounds.Item2;
		for (int x = item; x < maxX; x++)
		{
			for (int y = minY; y < maxY; y++)
			{
				yield return new Vector2Int(x, y);
			}
		}
	}

	public IEnumerable<Vector3Int> GetBlocksOnTerrainInRectangularRadius(int radius)
	{
		IEnumerable<Vector2Int> blocksInRectangularRadius = GetBlocksInRectangularRadius(radius);
		return _terrainAreaService.InMapCoordinates(blocksInRectangularRadius);
	}

	public IEnumerable<Vector3Int> GetBlocksOnTerrainOrStackableInRectangularRadius(int radius, bool finishedOnly)
	{
		IEnumerable<Vector2Int> blocksInRectangularRadius = GetBlocksInRectangularRadius(radius);
		return _stackableBlockService.GetGroundOrStackableBlocks(blocksInRectangularRadius, finishedOnly);
	}

	private Vector2Int RotatedSize()
	{
		Vector2Int result = _blockObject.Blocks.Size.XY();
		Orientation orientation = _blockObject.Orientation;
		if ((orientation != Orientation.Cw90 && orientation != Orientation.Cw270) || 1 == 0)
		{
			return result;
		}
		return new Vector2Int(result.y, result.x);
	}

	private static (int, int) GetAreaBounds(float center, int size, int radius)
	{
		int item = (int)(center - (float)size / 2f - (float)radius);
		int item2 = (int)(center + (float)size / 2f + (float)radius);
		return (item, item2);
	}
}
