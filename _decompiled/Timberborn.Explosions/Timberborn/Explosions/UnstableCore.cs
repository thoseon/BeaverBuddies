using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.ActivatorSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Debugging;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Explosions;

public class UnstableCore : BaseComponent, IActivableComponent, IAwakableComponent, IUpdatableComponent, IPersistentEntity, IInitializableEntity, IDeletableEntity, IDuplicable<UnstableCore>, IDuplicable
{
	private static readonly ComponentKey UnstableCoreKey = new ComponentKey("UnstableCore");

	private static readonly PropertyKey<int> ExplosionRadiusKey = new PropertyKey<int>("ExplosionRadius");

	private readonly MapEditorMode _mapEditorMode;

	private readonly DevModeManager _devModeManager;

	private readonly ExplosionService _explosionService;

	private UnstableCoreSpec _spec;

	private BlockObjectCenter _blockObjectCenter;

	private UnstableCoreEffectsSpawner _effectsSpawner;

	private UnstableCoreExplosionBlocker _explosionBlocker;

	private readonly HashSet<Vector3Int> _neighbouringTiles = new HashSet<Vector3Int>();

	private bool _triggered;

	private bool _delayedActivationEnabled;

	private float _remainingDelayedActivationTime;

	private bool _initialized;

	public int ExplosionRadius { get; private set; }

	public float InnerRadius => _spec.InnerRadius;

	public Vector3 ExplosionCenter => _blockObjectCenter.GridCenterGrounded;

	public bool IsDuplicable
	{
		get
		{
			if (!_mapEditorMode.IsMapEditor)
			{
				return _devModeManager.Enabled;
			}
			return true;
		}
	}

	public int MinExplosionRadius => _spec.MinExplosionRadius;

	public int MaxExplosionRadius => _spec.MaxExplosionRadius;

	public event EventHandler ExplosionRadiusChanged;

	public UnstableCore(MapEditorMode mapEditorMode, DevModeManager devModeManager, ExplosionService explosionService)
	{
		_mapEditorMode = mapEditorMode;
		_devModeManager = devModeManager;
		_explosionService = explosionService;
	}

	public void Awake()
	{
		_spec = GetComponent<UnstableCoreSpec>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_effectsSpawner = GetComponent<UnstableCoreEffectsSpawner>();
		_explosionBlocker = GetComponent<UnstableCoreExplosionBlocker>();
		ExplosionRadius = _spec.DefaultExplosionRadius;
	}

	public void Update()
	{
		if (_delayedActivationEnabled)
		{
			_remainingDelayedActivationTime -= Time.deltaTime;
			if (_remainingDelayedActivationTime <= 0f)
			{
				Activate();
			}
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(UnstableCoreKey).Set(ExplosionRadiusKey, ExplosionRadius);
	}

	[BackwardCompatible(2025, 9, 30, Compatibility.Map)]
	public void Load(IEntityLoader entityLoader)
	{
		if (!entityLoader.TryGetComponent(UnstableCoreKey, out var objectLoader))
		{
			objectLoader = entityLoader.GetComponent(new ComponentKey("TimeBomb"));
		}
		ExplosionRadius = objectLoader.Get(ExplosionRadiusKey);
	}

	public void InitializeEntity()
	{
		_explosionService.Register(this);
		InitializeNeighbouringTiles();
		_explosionService.TilesExplosion += OnTilesExplosion;
		_initialized = true;
		if (_triggered)
		{
			Explode();
		}
	}

	public void DuplicateFrom(UnstableCore source)
	{
		SetRadius(source.ExplosionRadius);
	}

	public void Deactivate()
	{
	}

	public void Activate()
	{
		TriggerExplosion();
	}

	public void SetRadius(int radius)
	{
		if (radius < MinExplosionRadius || radius > MaxExplosionRadius)
		{
			throw new ArgumentOutOfRangeException("radius", $"Explosion radius must be between {MinExplosionRadius} and {MaxExplosionRadius}.");
		}
		ExplosionRadius = radius;
		_explosionService.Register(this);
		ExplosionRadiusChanged?.Invoke(this, EventArgs.Empty);
	}

	public void DeleteEntity()
	{
		TriggerExplosion();
		_explosionService.TilesExplosion -= OnTilesExplosion;
	}

	public void ActivateDelayed(float delay)
	{
		_delayedActivationEnabled = true;
		_remainingDelayedActivationTime = delay;
	}

	private void InitializeNeighbouringTiles()
	{
		BlockObject component = GetComponent<BlockObject>();
		foreach (Vector3Int allCoordinate in component.PositionedBlocks.GetAllCoordinates())
		{
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int in neighbors4Vector3Int)
			{
				Vector3Int vector3Int2 = allCoordinate + vector3Int;
				if (!component.PositionedBlocks.HasBlockAt(vector3Int2))
				{
					_neighbouringTiles.Add(vector3Int2);
				}
			}
		}
	}

	private void OnTilesExplosion(object sender, ReadOnlyHashSet<Vector3Int> tiles)
	{
		if (_neighbouringTiles.Any(((ReadOnlyHashSet<Vector3Int>)tiles).Contains))
		{
			TriggerExplosion();
		}
	}

	private void TriggerExplosion()
	{
		if (!_triggered && !_explosionBlocker.ExplosionBlocked)
		{
			_triggered = true;
			if (_initialized)
			{
				Explode();
			}
		}
	}

	private void Explode()
	{
		_effectsSpawner.SpawnEffects();
		_explosionService.TilesExplosion -= OnTilesExplosion;
		_explosionService.Explode(this);
	}
}
