using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

internal class TerrainPhysicsUpdater : ITickableSingleton, IPostLoadableSingleton
{
	private readonly ITerrainService _terrainService;

	private readonly EntityService _entityService;

	private readonly TerrainDestroyer _terrainDestroyer;

	private readonly TerrainAndBlockObjectsToDeleteFinder _terrainAndBlockObjectsToDeleteFinder;

	private readonly HashSet<Vector3Int> _terrainQueuedForDestruction = new HashSet<Vector3Int>();

	private readonly HashSet<BlockObject> _blockObjectsToDelete = new HashSet<BlockObject>();

	private readonly Queue<Vector3Int> _terrainToCheck = new Queue<Vector3Int>();

	public TerrainPhysicsUpdater(ITerrainService terrainService, EntityService entityService, TerrainDestroyer terrainDestroyer, TerrainAndBlockObjectsToDeleteFinder terrainAndBlockObjectsToDeleteFinder)
	{
		_terrainService = terrainService;
		_entityService = entityService;
		_terrainDestroyer = terrainDestroyer;
		_terrainAndBlockObjectsToDeleteFinder = terrainAndBlockObjectsToDeleteFinder;
	}

	public void PostLoad()
	{
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	public void Tick()
	{
		if (_terrainToCheck.IsEmpty())
		{
			return;
		}
		_terrainAndBlockObjectsToDeleteFinder.FindAll(_terrainToCheck, _terrainQueuedForDestruction, _blockObjectsToDelete);
		_terrainToCheck.Clear();
		foreach (Vector3Int item in _terrainQueuedForDestruction)
		{
			_terrainDestroyer.DestroyTerrain(item);
		}
		foreach (BlockObject item2 in _blockObjectsToDelete)
		{
			_entityService.Delete(item2);
		}
		_terrainQueuedForDestruction.Clear();
		_blockObjectsToDelete.Clear();
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs preTerrainColumnChangedEventArgs)
	{
		TerrainHeightChange change = preTerrainColumnChangedEventArgs.Change;
		if (!change.SetTerrain)
		{
			for (int i = change.From; i <= change.To; i++)
			{
				Vector3Int item = change.Coordinates.ToVector3Int(i);
				_terrainToCheck.Enqueue(item);
			}
		}
	}
}
