using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

internal class TerrainOnBlockObjectFinder
{
	private readonly ITerrainService _terrainService;

	private readonly StackableBlockService _stackableBlockService;

	public TerrainOnBlockObjectFinder(ITerrainService terrainService, StackableBlockService stackableBlockService)
	{
		_terrainService = terrainService;
		_stackableBlockService = stackableBlockService;
	}

	public void Find(BlockObject blockObject, Queue<Vector3Int> terrainCoordinates)
	{
		foreach (Block occupiedBlock in blockObject.PositionedBlocks.GetOccupiedBlocks())
		{
			Vector3Int coordinates = occupiedBlock.Coordinates;
			Vector3Int vector3Int = coordinates.Above();
			if (IsUnderground(vector3Int) && blockObject.IsFinished && occupiedBlock.Stackable.IsStackable())
			{
				terrainCoordinates.Enqueue(vector3Int);
			}
			if (!blockObject.IsUnfinished || !occupiedBlock.Stackable.IsUnfinishedGround())
			{
				continue;
			}
			if (IsUnderground(vector3Int))
			{
				terrainCoordinates.Enqueue(vector3Int);
			}
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int2 in neighbors4Vector3Int)
			{
				Vector3Int vector3Int3 = coordinates + vector3Int2;
				if (IsUnderground(vector3Int3))
				{
					terrainCoordinates.Enqueue(vector3Int3);
				}
			}
		}
	}

	private bool IsUnderground(Vector3Int coordinates)
	{
		if (!_terrainService.Underground(coordinates))
		{
			return _stackableBlockService.IsUnfinishedGroundBlockAt(coordinates);
		}
		return true;
	}
}
