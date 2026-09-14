using System.Collections.Generic;
using System.Linq;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockOccupancyService : IBlockOccupancyService
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	public BlockOccupancyService(EntityComponentRegistry entityComponentRegistry)
	{
		_entityComponentRegistry = entityComponentRegistry;
	}

	public bool OccupantPresentOnArea(BlockObject blockObject, float minDistanceFromArea)
	{
		IEnumerable<BlockOccupant> occupants = _entityComponentRegistry.GetEnabled<BlockOccupant>();
		return blockObject.PositionedBlocks.GetOccupiedCoordinates().Any((Vector3Int coords) => occupants.Any((BlockOccupant beaver) => OccupantAtCoords(beaver, coords, minDistanceFromArea)));
	}

	private static bool OccupantAtCoords(BlockOccupant occupant, Vector3Int coordinates, float minDistanceFromTile)
	{
		float num = (float)coordinates.x - minDistanceFromTile;
		float num2 = (float)(coordinates.x + 1) + minDistanceFromTile;
		float num3 = (float)coordinates.y - minDistanceFromTile;
		float num4 = (float)(coordinates.y + 1) + minDistanceFromTile;
		Vector3 gridCoordinates = occupant.GridCoordinates;
		float x = gridCoordinates.x;
		float y = gridCoordinates.y;
		int num5 = Mathf.FloorToInt(gridCoordinates.z);
		if (x >= num && x <= num2 && y >= num3 && y <= num4)
		{
			return coordinates.z == num5;
		}
		return false;
	}
}
