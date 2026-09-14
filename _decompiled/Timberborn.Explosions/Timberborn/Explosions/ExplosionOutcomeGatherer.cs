using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.MapStateSystem;
using Timberborn.TerrainPhysics;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Explosions;

public class ExplosionOutcomeGatherer
{
	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private readonly MapSize _mapSize;

	private readonly HashSet<Vector3Int> _additionalTerrain = new HashSet<Vector3Int>();

	private readonly HashSet<BlockObject> _additionalBlockObjects = new HashSet<BlockObject>();

	public ExplosionOutcomeGatherer(ITerrainPhysicsService terrainPhysicsService, ITerrainService terrainService, IBlockService blockService, MapSize mapSize)
	{
		_terrainPhysicsService = terrainPhysicsService;
		_terrainService = terrainService;
		_blockService = blockService;
		_mapSize = mapSize;
	}

	public void GetAllAffectedTerrainAndObjects(UnstableCore unstableCore, HashSet<Vector3Int> affectedTiles, HashSet<Vector3Int> affectedTerrain, HashSet<BlockObject> affectedObjects)
	{
		GetAffectedTiles(unstableCore, affectedTiles);
		GetAffectedTerrainAndObjects(affectedTiles.AsReadOnlyHashSet(), affectedTerrain, affectedObjects);
	}

	public Dictionary<int, HashSet<Vector3Int>> GetAffectedTilesPerRadius(Vector3 center, float radius)
	{
		Dictionary<int, HashSet<Vector3Int>> dictionary = new Dictionary<int, HashSet<Vector3Int>>();
		foreach (var item2 in GetCoordinatesInRadiusWithDistance(center, radius))
		{
			Vector3Int item = item2.Item1;
			int num = Mathf.Max(Mathf.FloorToInt(item2.Item2), 0);
			AddCoordinatesToRadiusGroup(dictionary, num, item);
		}
		return dictionary;
	}

	public void GetAffectedTerrainAndObjects(ReadOnlyHashSet<Vector3Int> affectedTiles, HashSet<Vector3Int> affectedTerrain, HashSet<BlockObject> affectedObjects)
	{
		foreach (Vector3Int item in affectedTiles)
		{
			if (_terrainService.Underground(item))
			{
				affectedTerrain.Add(item);
			}
			foreach (BlockObject item2 in _blockService.GetObjectsAt(item))
			{
				if (!item2.HasComponent<INonStackPickable>() && item2.HasComponent<EntityComponent>())
				{
					affectedObjects.Add(item2);
				}
			}
		}
		AddObjectsOnTopOfTerrain(affectedTiles, affectedTerrain, affectedObjects);
		ApplyTerrainPhysics(affectedTerrain, affectedObjects);
	}

	private void GetAffectedTiles(UnstableCore unstableCore, HashSet<Vector3Int> affectedTiles)
	{
		float radius = (float)unstableCore.ExplosionRadius + unstableCore.InnerRadius;
		Vector3 explosionCenter = unstableCore.ExplosionCenter;
		foreach (var item2 in GetCoordinatesInRadiusWithDistance(explosionCenter, radius))
		{
			Vector3Int item = item2.Item1;
			if (_mapSize.ContainsInTotal(item))
			{
				affectedTiles.Add(item);
			}
		}
	}

	private void ApplyTerrainPhysics(HashSet<Vector3Int> affectedTerrain, HashSet<BlockObject> affectedObjects)
	{
		_terrainPhysicsService.GetTerrainAndBlockObjectStack(affectedTerrain, affectedObjects, _additionalTerrain, _additionalBlockObjects);
		affectedTerrain.AddRange(_additionalTerrain);
		affectedObjects.AddRange(_additionalBlockObjects);
		_additionalTerrain.Clear();
		_additionalBlockObjects.Clear();
	}

	private IEnumerable<(Vector3Int, float)> GetCoordinatesInRadiusWithDistance(Vector3 center, float radius)
	{
		int num = Mathf.FloorToInt(center.x - radius);
		int maxX = Mathf.CeilToInt(center.x + radius);
		int minY = Mathf.FloorToInt(center.y - radius);
		int maxY = Mathf.CeilToInt(center.y + radius);
		int minZ = Mathf.FloorToInt(center.z - radius);
		int maxZ = Mathf.CeilToInt(center.z + radius);
		for (int x = num; x <= maxX; x++)
		{
			for (int y = minY; y <= maxY; y++)
			{
				for (int z = minZ; z <= maxZ; z++)
				{
					float magnitude = (new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f) - center).magnitude;
					if (magnitude <= radius)
					{
						yield return (new Vector3Int(x, y, z), magnitude);
					}
				}
			}
		}
	}

	private void AddCoordinatesToRadiusGroup(Dictionary<int, HashSet<Vector3Int>> affectedTilesPerRadius, int group, Vector3Int coordinates)
	{
		if (!affectedTilesPerRadius.TryGetValue(group, out var value))
		{
			value = (affectedTilesPerRadius[group] = new HashSet<Vector3Int>());
		}
		value.Add(coordinates);
	}

	private void AddObjectsOnTopOfTerrain(ReadOnlyHashSet<Vector3Int> affectedTiles, HashSet<Vector3Int> affectedTerrain, HashSet<BlockObject> affectedObjects)
	{
		foreach (Vector3Int item in affectedTerrain)
		{
			Vector3Int vector3Int = item.Above();
			if (!affectedTiles.Contains(vector3Int) && !_terrainService.Underground(vector3Int))
			{
				GetAllObjectsOnTerrain(affectedObjects, vector3Int);
			}
		}
	}

	private void GetAllObjectsOnTerrain(HashSet<BlockObject> affectedObjects, Vector3Int tileAbove)
	{
		foreach (BlockObject item in _blockService.GetObjectsAt(tileAbove))
		{
			if (!item.HasComponent<INonStackPickable>())
			{
				MatterBelow matterBelow = item.PositionedBlocks.GetBlock(tileAbove).MatterBelow;
				if ((uint)matterBelow <= 1u)
				{
					affectedObjects.Add(item);
				}
			}
		}
	}
}
