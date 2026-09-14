using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

public class TerrainDestroyer
{
	private readonly ITerrainService _terrainService;

	private readonly EventBus _eventBus;

	public TerrainDestroyer(ITerrainService terrainService, EventBus eventBus)
	{
		_terrainService = terrainService;
		_eventBus = eventBus;
	}

	public void DestroyTerrain(Vector3Int coordinates)
	{
		_terrainService.UnsetTerrain(coordinates);
		_eventBus.Post(new TerrainDestroyedEvent(coordinates));
	}
}
