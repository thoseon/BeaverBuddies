using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

internal class TerrainPhysicsService : ITerrainPhysicsService, ILoadableSingleton
{
	private readonly TerrainPhysicsValidatorFactory _terrainPhysicsValidatorFactory;

	private readonly TerrainAndBlockObjectsToDeleteFinder _terrainAndBlockObjectsToDeleteFinder;

	private TerrainPhysicsValidator _terrainPhysicsValidator;

	private TerrainPhysicsValidator _previewTerrainPhysicsValidator;

	public ReadOnlyList<Vector3Int> PhysicsSupportDeltas { get; private set; }

	public TerrainPhysicsService(TerrainPhysicsValidatorFactory terrainPhysicsValidatorFactory, TerrainAndBlockObjectsToDeleteFinder terrainAndBlockObjectsToDeleteFinder)
	{
		_terrainPhysicsValidatorFactory = terrainPhysicsValidatorFactory;
		_terrainAndBlockObjectsToDeleteFinder = terrainAndBlockObjectsToDeleteFinder;
	}

	public void Load()
	{
		_terrainPhysicsValidator = _terrainPhysicsValidatorFactory.CreateValidator();
		_previewTerrainPhysicsValidator = _terrainPhysicsValidatorFactory.CreatePreviewValidator();
		PhysicsSupportDeltas = GetCheckAreaCoordinates().AsReadOnlyList();
	}

	public void GetTerrainAndBlockObjectStack(IEnumerable<BlockObject> inputBlockObjects, HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects)
	{
		_terrainAndBlockObjectsToDeleteFinder.FindAll(inputBlockObjects, outputTerrain, outputBlockObjects);
	}

	public void GetTerrainAndBlockObjectStack(IEnumerable<Vector3Int> inputTerrain, IEnumerable<BlockObject> inputBlockObjects, HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects)
	{
		_terrainAndBlockObjectsToDeleteFinder.FindAllMarkInputAsDeleted(inputTerrain, inputBlockObjects, outputTerrain, outputBlockObjects);
	}

	public void GetValidTerrainToAdd(ICollection<Vector3Int> inputTerrain, HashSet<Vector3Int> terrainToAdd)
	{
		_terrainPhysicsValidator.GetValidTerrainToAdd(inputTerrain, terrainToAdd);
	}

	public bool CanBeDestroyed(BlockObject blockObject)
	{
		return _previewTerrainPhysicsValidator.CanBeDestroyed(blockObject);
	}

	public bool ValidateBlockObjectPreview(BlockObject blockObject)
	{
		return _previewTerrainPhysicsValidator.ValidateBlockObjectPreview(blockObject);
	}

	public bool CanTerrainBeAdded(Vector3Int coordinates)
	{
		return _terrainPhysicsValidator.CanTerrainBeAdded(coordinates);
	}

	private static List<Vector3Int> GetCheckAreaCoordinates()
	{
		List<Vector3Int> list = new List<Vector3Int>();
		int maxSupportDistance = TerrainPhysicsValidator.MaxSupportDistance;
		for (int i = -maxSupportDistance; i <= maxSupportDistance; i++)
		{
			int num = Mathf.Abs(i);
			for (int j = -maxSupportDistance; j <= maxSupportDistance; j++)
			{
				if (Mathf.Abs(j) + num <= maxSupportDistance)
				{
					if (i != 0 || j != 0)
					{
						list.Add(new Vector3Int(j, i, 0));
					}
					list.Add(new Vector3Int(j, i, 1));
				}
			}
		}
		return list;
	}
}
