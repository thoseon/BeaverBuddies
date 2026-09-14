using System;
using System.Collections.Generic;
using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TerrainPhysics;
using UnityEngine;

namespace Timberborn.DeconstructionSystem;

public class DeconstructionParticleFactory : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly struct ParticleSpawnParameters : IEquatable<ParticleSpawnParameters>
	{
		public Vector3Int Coordinates { get; }

		public bool UseUnscaledTime { get; }

		public ParticleSpawnParameters(Vector3Int coordinates, bool useUnscaledTime)
		{
			Coordinates = coordinates;
			UseUnscaledTime = useUnscaledTime;
		}

		public bool Equals(ParticleSpawnParameters other)
		{
			if (Coordinates.Equals(other.Coordinates))
			{
				return UseUnscaledTime == other.UseUnscaledTime;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is ParticleSpawnParameters other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Coordinates, UseUnscaledTime);
		}
	}

	private readonly EventBus _eventBus;

	private readonly IInstantiator _instantiator;

	private readonly IAssetLoader _assetLoader;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly ISpecService _specService;

	private DeconstructionParticleFactorySpec _spec;

	private readonly Dictionary<Vector3Int, byte> _particlesInNeighbours = new Dictionary<Vector3Int, byte>();

	private readonly HashSet<ParticleSpawnParameters> _particlesToSpawn = new HashSet<ParticleSpawnParameters>();

	private GameObject _particlePrefab;

	private Transform _root;

	public DeconstructionParticleFactory(EventBus eventBus, IInstantiator instantiator, IAssetLoader assetLoader, RootObjectProvider rootObjectProvider, IRandomNumberGenerator randomNumberGenerator, ISpecService specService)
	{
		_eventBus = eventBus;
		_instantiator = instantiator;
		_assetLoader = assetLoader;
		_rootObjectProvider = rootObjectProvider;
		_randomNumberGenerator = randomNumberGenerator;
		_specService = specService;
	}

	public void Load()
	{
		_spec = _specService.GetSingleSpec<DeconstructionParticleFactorySpec>();
		_root = _rootObjectProvider.CreateRootObject("DeconstructionParticleFactory").transform;
		_particlePrefab = _assetLoader.Load<GameObject>(_spec.ParticlePrefabPath);
		_eventBus.Register(this);
	}

	public void AddPausableParticles(HashSet<Vector3Int> particlesCoordinates)
	{
		foreach (Vector3Int particlesCoordinate in particlesCoordinates)
		{
			AddParticles(particlesCoordinate, useUnscaledTime: false);
		}
	}

	[OnEvent]
	public void OnBuildingDeconstructed(BuildingDeconstructedEvent buildingDeconstructedEvent)
	{
		foreach (Vector3Int coordinate in buildingDeconstructedEvent.Coordinates)
		{
			AddParticles(coordinate);
		}
	}

	[OnEvent]
	public void OnTerrainDestroyed(TerrainDestroyedEvent terrainDestroyedEvent)
	{
		AddParticles(terrainDestroyedEvent.Coordinates);
	}

	public void LateUpdateSingleton()
	{
		if (_particlesToSpawn.Count > 0)
		{
			if (_particlesToSpawn.Count <= _spec.MinParticlesForThreshold)
			{
				SpawnAllParticles();
			}
			else
			{
				SpawnLimitedParticles();
			}
			_particlesToSpawn.Clear();
			_particlesInNeighbours.Clear();
		}
	}

	private void AddParticles(Vector3Int coordinates, bool useUnscaledTime = true)
	{
		if (!_particlesToSpawn.Add(new ParticleSpawnParameters(coordinates, useUnscaledTime)))
		{
			return;
		}
		Vector3Int[] neighbors26Vector3Int = Deltas.Neighbors26Vector3Int;
		foreach (Vector3Int vector3Int in neighbors26Vector3Int)
		{
			Vector3Int key = coordinates + vector3Int;
			if (_particlesInNeighbours.TryGetValue(key, out var value))
			{
				_particlesInNeighbours[key] = (byte)(value + 1);
			}
			else
			{
				_particlesInNeighbours[key] = 1;
			}
		}
	}

	private void SpawnAllParticles()
	{
		foreach (ParticleSpawnParameters item in _particlesToSpawn)
		{
			SpawnParticle(item);
		}
	}

	private void SpawnLimitedParticles()
	{
		float countFactor = Mathf.Clamp01((float)(_particlesToSpawn.Count - _spec.MinParticlesForThreshold) / (float)(_spec.MaxParticlesForThreshold - _spec.MinParticlesForThreshold));
		foreach (ParticleSpawnParameters item in _particlesToSpawn)
		{
			if (ShouldSpawnParticle(item, countFactor))
			{
				SpawnParticle(item);
			}
		}
	}

	private bool ShouldSpawnParticle(ParticleSpawnParameters spawnParameters, float countFactor)
	{
		if (_particlesInNeighbours.TryGetValue(spawnParameters.Coordinates, out var value))
		{
			float num = Mathf.Lerp(_spec.MinParticleSpawnThreshold, _spec.MaxParticleSpawnThreshold, Mathf.Sqrt((float)(int)value / _spec.MaxNeighboursCount));
			return _randomNumberGenerator.Range(0f, 1f) > num * countFactor;
		}
		return true;
	}

	private void SpawnParticle(ParticleSpawnParameters spawnParameters)
	{
		GameObject gameObject = _instantiator.Instantiate(_particlePrefab, _root);
		gameObject.transform.position = CoordinateSystem.GridToWorld(spawnParameters.Coordinates);
		if (!spawnParameters.UseUnscaledTime)
		{
			ParticleSystem.MainModule main = gameObject.GetComponent<ParticleSystem>().main;
			main.useUnscaledTime = false;
		}
	}
}
