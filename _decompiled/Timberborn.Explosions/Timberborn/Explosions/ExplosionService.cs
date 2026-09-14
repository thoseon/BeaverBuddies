using System;
using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DeconstructionSystem;
using Timberborn.EntitySystem;
using Timberborn.MapEditorTickSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Explosions;

[MapEditorTickable]
public class ExplosionService : ILoadableSingleton, ISaveableSingleton, ITickableSingleton
{
	private static readonly SingletonKey ExplosionServiceKey = new SingletonKey("ExplosionService");

	private static readonly ListKey<ExplosionData> ExplosionsKey = new ListKey<ExplosionData>("Explosions");

	private readonly ISingletonLoader _singletonLoader;

	private readonly ExplosionDataValueSerializer _explosionDataValueSerializer;

	private readonly ExplosionOutcomeGatherer _explosionOutcomeGatherer;

	private readonly ITerrainService _terrainService;

	private readonly EntityService _entityService;

	private readonly CharacterExploder _characterExploder;

	private readonly DeconstructionParticleFactory _deconstructionParticleFactory;

	private readonly Dictionary<UnstableCore, ExplosionData> _registeredCores = new Dictionary<UnstableCore, ExplosionData>();

	private readonly List<ExplosionData> _explosions = new List<ExplosionData>();

	private readonly HashSet<Vector3Int> _terrainTiles = new HashSet<Vector3Int>();

	private readonly HashSet<BlockObject> _blockObjects = new HashSet<BlockObject>();

	public event EventHandler<ReadOnlyHashSet<Vector3Int>> TilesExplosion;

	public ExplosionService(ISingletonLoader singletonLoader, ExplosionDataValueSerializer explosionDataValueSerializer, ExplosionOutcomeGatherer explosionOutcomeGatherer, ITerrainService terrainService, EntityService entityService, CharacterExploder characterExploder, DeconstructionParticleFactory deconstructionParticleFactory)
	{
		_singletonLoader = singletonLoader;
		_explosionDataValueSerializer = explosionDataValueSerializer;
		_explosionOutcomeGatherer = explosionOutcomeGatherer;
		_terrainService = terrainService;
		_entityService = entityService;
		_characterExploder = characterExploder;
		_deconstructionParticleFactory = deconstructionParticleFactory;
	}

	public void Load()
	{
		if (!_singletonLoader.TryGetSingleton(ExplosionServiceKey, out var objectLoader))
		{
			return;
		}
		foreach (ExplosionData item in objectLoader.Get(ExplosionsKey, _explosionDataValueSerializer))
		{
			item.InitializeAffectedTiles(_explosionOutcomeGatherer);
			_explosions.Add(item);
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (_explosions.Count > 0)
		{
			singletonSaver.GetSingleton(ExplosionServiceKey).Set(ExplosionsKey, _explosions, _explosionDataValueSerializer);
		}
	}

	public void Tick()
	{
		for (int num = _explosions.Count - 1; num >= 0; num--)
		{
			ExplosionData explosionData = _explosions[num];
			if (explosionData.TryGetExplosionOutcomeForCurrentRadius(out var readOnlyAffectedTiles))
			{
				ProcessAffectedTiles(readOnlyAffectedTiles);
				TilesExplosion?.Invoke(this, readOnlyAffectedTiles);
				if (!explosionData.MoveToNextRadius())
				{
					_explosions.RemoveAt(num);
				}
			}
		}
	}

	public void Register(UnstableCore unstableCore)
	{
		_registeredCores.Remove(unstableCore);
		_registeredCores.Add(unstableCore, GenerateExplosionData(unstableCore));
	}

	public void Explode(UnstableCore unstableCore)
	{
		if (_registeredCores.TryGetValue(unstableCore, out var value))
		{
			_explosions.Add(value);
			_registeredCores.Remove(unstableCore);
		}
	}

	private void ProcessAffectedTiles(ReadOnlyHashSet<Vector3Int> affectedTiles)
	{
		_explosionOutcomeGatherer.GetAffectedTerrainAndObjects(affectedTiles, _terrainTiles, _blockObjects);
		_characterExploder.ExplodeCharactersAt(affectedTiles, null);
		DestroyEverythingAffected();
		_terrainTiles.Clear();
		_blockObjects.Clear();
	}

	private ExplosionData GenerateExplosionData(UnstableCore unstableCore)
	{
		ExplosionData explosionData = new ExplosionData((float)unstableCore.ExplosionRadius + unstableCore.InnerRadius, unstableCore.ExplosionCenter);
		explosionData.InitializeAffectedTiles(_explosionOutcomeGatherer);
		return explosionData;
	}

	private void DestroyEverythingAffected()
	{
		_terrainService.UnsetTerrain(_terrainTiles);
		foreach (BlockObject blockObject in _blockObjects)
		{
			_entityService.Delete(blockObject);
		}
		_deconstructionParticleFactory.AddPausableParticles(_terrainTiles);
	}
}
