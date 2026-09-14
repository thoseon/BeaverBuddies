using System.Collections.Generic;
using System.Linq;
using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.DeconstructionSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Explosions;

public class Dynamite : TickableComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity, IInitializableEntity, ITerrainRemovingEntity
{
	private static readonly ComponentKey DynamiteKey = new ComponentKey("Dynamite");

	private static readonly PropertyKey<bool> IsTriggeredKey = new PropertyKey<bool>("IsTriggered");

	private static readonly PropertyKey<int> TickCounterKey = new PropertyKey<int>("TickCounter");

	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private readonly EntityService _entityService;

	private readonly IInstantiator _instantiator;

	private readonly EventBus _eventBus;

	private readonly IAssetLoader _assetLoader;

	private readonly ExplosionSoundPlayer _explosionSoundPlayer;

	private readonly ExplosionService _explosionService;

	private readonly CharacterExploder _characterExploder;

	private BlockObject _blockObject;

	private DynamiteSpec _dynamiteSpec;

	private GameObject _explosionPrefab;

	private readonly HashSet<Vector3Int> _neighbouringTiles = new HashSet<Vector3Int>();

	private int _tickCounter;

	private int _ticksToDetonate;

	public bool IsTriggered { get; private set; }

	public int Depth => _dynamiteSpec.Depth;

	public bool IsFinished => _blockObject.IsFinished;

	public Dynamite(ITerrainService terrainService, IBlockService blockService, EntityService entityService, IInstantiator instantiator, EventBus eventBus, IAssetLoader assetLoader, ExplosionSoundPlayer explosionSoundPlayer, ExplosionService explosionService, CharacterExploder characterExploder)
	{
		_terrainService = terrainService;
		_blockService = blockService;
		_entityService = entityService;
		_instantiator = instantiator;
		_eventBus = eventBus;
		_assetLoader = assetLoader;
		_explosionSoundPlayer = explosionSoundPlayer;
		_explosionService = explosionService;
		_characterExploder = characterExploder;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_dynamiteSpec = GetComponent<DynamiteSpec>();
		_explosionPrefab = _assetLoader.Load<GameObject>(_dynamiteSpec.ExplosionPrefabPath);
		DisableComponent();
	}

	public override void Tick()
	{
		if (IsTriggered && ++_tickCounter > _ticksToDetonate)
		{
			Detonate();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsTriggered)
		{
			IObjectSaver component = entitySaver.GetComponent(DynamiteKey);
			component.Set(IsTriggeredKey, IsTriggered);
			component.Set(TickCounterKey, _tickCounter);
		}
	}

	[BackwardCompatible(2026, 2, 9, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DynamiteKey, out var objectLoader))
		{
			IsTriggered = (objectLoader.Has(IsTriggeredKey) ? objectLoader.Get(IsTriggeredKey) : objectLoader.Get(new PropertyKey<bool>("Triggered")));
			_tickCounter = objectLoader.Get(TickCounterKey);
		}
	}

	public void InitializeEntity()
	{
		InitializeNeighbouringTiles();
		_explosionService.TilesExplosion += OnTilesExplosion;
		if (IsTriggered)
		{
			EnableComponent();
		}
	}

	public void Trigger()
	{
		if (_blockObject.IsFinished)
		{
			_ticksToDetonate = 1;
			IsTriggered = true;
			EnableComponent();
		}
	}

	public void TriggerDelayed(int delayInTicks)
	{
		if (_blockObject.IsFinished)
		{
			_ticksToDetonate = delayInTicks;
			IsTriggered = true;
			EnableComponent();
		}
	}

	public void Disarm()
	{
		IsTriggered = false;
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_explosionService.TilesExplosion -= OnTilesExplosion;
	}

	public bool RemovesTerrainAt(Vector3Int coordinates)
	{
		Vector3Int vector3Int = _blockObject.Coordinates.Below();
		int num = CalculateEffectiveDepth(vector3Int);
		for (int i = 0; i < num; i++)
		{
			if (vector3Int == coordinates)
			{
				return true;
			}
			vector3Int.z--;
		}
		return false;
	}

	private void Detonate()
	{
		TriggerNeighbors();
		DestroyPathBlockObject();
		LowerTerrainBelow();
		_characterExploder.ExplodeCharactersAt(_blockObject.Coordinates, this);
		PlayEffects();
		GetComponent<Deconstructible>().DisableDeconstruction();
		_entityService.Delete(this);
		IsTriggered = false;
		_eventBus.Post(new DynamiteDetonatedEvent());
	}

	private void InitializeNeighbouringTiles()
	{
		foreach (Vector3Int allCoordinate in GetComponent<BlockObject>().PositionedBlocks.GetAllCoordinates())
		{
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int in neighbors4Vector3Int)
			{
				_neighbouringTiles.Add(allCoordinate + vector3Int);
			}
		}
	}

	private void OnTilesExplosion(object sender, ReadOnlyHashSet<Vector3Int> tiles)
	{
		if (IsFinished && _neighbouringTiles.Any(((ReadOnlyHashSet<Vector3Int>)tiles).Contains))
		{
			Trigger();
		}
	}

	private void TriggerNeighbors()
	{
		Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
		foreach (Vector3Int vector3Int in neighbors4Vector3Int)
		{
			Vector3Int coordinates = _blockObject.Coordinates + vector3Int;
			Dynamite bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Dynamite>(coordinates);
			if ((bool)bottomObjectComponentAt && bottomObjectComponentAt.IsFinished)
			{
				bottomObjectComponentAt.Trigger();
			}
			UnstableCore bottomObjectComponentAt2 = _blockService.GetBottomObjectComponentAt<UnstableCore>(coordinates);
			if ((bool)bottomObjectComponentAt2)
			{
				bottomObjectComponentAt2.Activate();
			}
		}
	}

	private void DestroyPathBlockObject()
	{
		BlockObject pathObjectAt = _blockService.GetPathObjectAt(_blockObject.Coordinates);
		if (pathObjectAt != null)
		{
			pathObjectAt.GetComponent<Deconstructible>().DisableDeconstruction();
			_entityService.Delete(pathObjectAt);
		}
	}

	private void LowerTerrainBelow()
	{
		Vector3Int coordinates = _blockObject.Coordinates.Below();
		int heightChange = CalculateEffectiveDepth(coordinates);
		_terrainService.UnsetTerrain(coordinates, heightChange);
	}

	private int CalculateEffectiveDepth(Vector3Int coordinates)
	{
		Vector2Int coords2D = coordinates.XY();
		int z = coordinates.z;
		for (int i = 0; i < Depth; i++)
		{
			if (_blockService.AnyObjectAt(coords2D.ToVector3Int(z - i)))
			{
				return i;
			}
		}
		return Depth;
	}

	private void PlayEffects()
	{
		GameObject gameObject = _instantiator.Instantiate(_explosionPrefab, null);
		gameObject.transform.position = _blockObject.GetComponent<BlockObjectCenter>().WorldCenter;
		_explosionSoundPlayer.Play(gameObject);
	}
}
