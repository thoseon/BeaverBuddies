using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public class WaterInputPipeCoordinates : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, IPostPlacementChangeListener, IPostInitializableEntity, IPersistentEntity, IWaterInputCoordinates, IDuplicable<WaterInputPipeCoordinates>, IDuplicable
{
	public static readonly BlockOccupations InvalidOccupations = ~(BlockOccupations.Floor | BlockOccupations.Corners);

	private static readonly ComponentKey ComponentKey = new ComponentKey("WaterInputPipeCoordinates");

	private static readonly PropertyKey<int> DepthLimitKey = new PropertyKey<int>("DepthLimit");

	private static readonly PropertyKey<bool> UseDepthLimitKey = new PropertyKey<bool>("UseDepthLimit");

	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private readonly EventBus _eventBus;

	private BlockObject _blockObject;

	private WaterInputSpec _waterInputSpec;

	private WaterInputPipeSpec _waterInputPipeSpec;

	private readonly List<BlockObject> _blockObjectCache = new List<BlockObject>();

	public Vector3Int Coordinates { get; private set; }

	public int Depth { get; private set; }

	public int DepthLimit { get; private set; }

	public bool UseDepthLimit { get; private set; }

	public bool IsBlocked => Depth == 0;

	public event EventHandler<Vector3Int> CoordinatesChanged;

	public WaterInputPipeCoordinates(ITerrainService terrainService, IBlockService blockService, EventBus eventBus)
	{
		_terrainService = terrainService;
		_blockService = blockService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_waterInputSpec = GetComponent<WaterInputSpec>();
		_waterInputPipeSpec = GetComponent<WaterInputPipeSpec>();
		DepthLimit = _waterInputPipeSpec.MaxDepth;
		Asserts.ValueIsInRange(DepthLimit, 1, int.MaxValue, "DepthLimit");
	}

	public void InitializeEntity()
	{
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
		_eventBus.Register(this);
	}

	public void PostInitializeEntity()
	{
		UpdateCoordinatesAndDepth();
	}

	public void DeleteEntity()
	{
		_terrainService.TerrainHeightChanged -= OnTerrainHeightChanged;
		_eventBus.Unregister(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(DepthLimitKey, DepthLimit);
		component.Set(UseDepthLimitKey, UseDepthLimit);
	}

	[BackwardCompatible(2026, 3, 4, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (!entityLoader.TryGetComponent(new ComponentKey("WaterInputCoordinates"), out var objectLoader))
		{
			objectLoader = entityLoader.GetComponent(ComponentKey);
		}
		DepthLimit = Math.Min(objectLoader.Get(DepthLimitKey), _waterInputPipeSpec.MaxDepth);
		UseDepthLimit = objectLoader.Get(UseDepthLimitKey);
	}

	public void DuplicateFrom(WaterInputPipeCoordinates source)
	{
		UseDepthLimit = source.UseDepthLimit;
		DepthLimit = Math.Min(source.DepthLimit, _waterInputPipeSpec.MaxDepth);
		UpdateCoordinatesAndDepth();
	}

	public void OnPostPlacementChanged()
	{
		UpdateCoordinatesAndDepth();
	}

	public void SetDepthLimit(int depth)
	{
		UseDepthLimit = true;
		DepthLimit = depth;
		UpdateCoordinatesAndDepth();
	}

	public void DisableDepthLimit()
	{
		UseDepthLimit = false;
		DepthLimit = _waterInputPipeSpec.MaxDepth;
		UpdateCoordinatesAndDepth();
	}

	[OnEvent]
	public void OnBlockObjectSetEvent(BlockObjectSetEvent blockObjectSetEvent)
	{
		if (ShouldUpdate(blockObjectSetEvent.BlockObject.PositionedBlocks))
		{
			UpdateCoordinatesAndDepth();
		}
	}

	[OnEvent]
	public void OnBlockObjectUnsetEvent(BlockObjectUnsetEvent blockObjectUnsetEvent)
	{
		if (ShouldUpdate(blockObjectUnsetEvent.BlockObject.PositionedBlocks))
		{
			UpdateCoordinatesAndDepth();
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		if (terrainHeightChangeEventArgs.Change.Coordinates == Coordinates.XY())
		{
			UpdateCoordinatesAndDepth();
		}
	}

	private bool ShouldUpdate(PositionedBlocks changedBlocks)
	{
		foreach (Vector3Int allCoordinate in changedBlocks.GetAllCoordinates())
		{
			if (allCoordinate.XY() == Coordinates.XY() && allCoordinate.z <= _blockObject.CoordinatesAtBaseZ.z)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateCoordinatesAndDepth()
	{
		Vector3Int waterInputCoordinates = _waterInputSpec.WaterInputCoordinates;
		Vector3Int startCoordinates = _blockObject.TransformCoordinates(waterInputCoordinates) + new Vector3Int(0, 0, _blockObject.BaseZ + 1);
		Coordinates = new Vector3Int(startCoordinates.x, startCoordinates.y, GetZCoordinateLimitedByDepth(startCoordinates));
		Depth = startCoordinates.z - Coordinates.z;
		CoordinatesChanged?.Invoke(this, Coordinates);
	}

	private int GetZCoordinateLimitedByDepth(Vector3Int startCoordinates)
	{
		int z = startCoordinates.z;
		int num = (UseDepthLimit ? DepthLimit : _waterInputPipeSpec.MaxDepth);
		for (int num2 = z - 1; num2 >= z - num; num2--)
		{
			Vector3Int coordinates = new Vector3Int(startCoordinates.x, startCoordinates.y, num2);
			if (IsTileOccupied(coordinates))
			{
				return num2 + 1;
			}
		}
		return z - num;
	}

	private bool IsTileOccupied(Vector3Int coordinates)
	{
		if (_terrainService.Underground(coordinates))
		{
			return true;
		}
		_blockService.GetIntersectingObjectsAt(coordinates, InvalidOccupations, _blockObjectCache);
		bool result = HasOccupyingObject();
		_blockObjectCache.Clear();
		return result;
	}

	private bool HasOccupyingObject()
	{
		foreach (BlockObject item in _blockObjectCache)
		{
			if (!item.Overridable && item != _blockObject && !item.HasComponent<PipeIntersectionAllowerSpec>())
			{
				return true;
			}
		}
		return false;
	}
}
