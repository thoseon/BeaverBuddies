using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.MapStateSystem;
using Timberborn.NaturalResources;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Planting;

public class PlantingService : ISaveableSingleton, ILoadableSingleton, IPostLoadableSingleton
{
	private static readonly SingletonKey PlantingServiceKey = new SingletonKey("PlantingService");

	private static readonly PropertyKey<PlantingMap> PlantingMapKey = new PropertyKey<PlantingMap>("PlantingMap");

	private readonly ISingletonLoader _singletonLoader;

	private readonly ITerrainService _terrainService;

	private readonly EventBus _eventBus;

	private readonly SpawnValidationService _spawnValidationService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly PlantingMapSerializer _plantingMapSerializer;

	private readonly IBlockService _blockService;

	private PlantingMap _plantingMap;

	private readonly HashSet<Vector3Int> _reservedCoordinates = new HashSet<Vector3Int>();

	private readonly Dictionary<Vector3Int, PlantingSpot> _plantingSpots = new Dictionary<Vector3Int, PlantingSpot>();

	public IEnumerable<Vector3Int> PlantingCoordinates => _plantingMap.GetCoordinatesWithSetResource();

	public PlantingService(ISingletonLoader singletonLoader, ITerrainService terrainService, EventBus eventBus, SpawnValidationService spawnValidationService, MapEditorMode mapEditorMode, PlantingMapSerializer plantingMapSerializer, IBlockService blockService)
	{
		_singletonLoader = singletonLoader;
		_terrainService = terrainService;
		_eventBus = eventBus;
		_spawnValidationService = spawnValidationService;
		_mapEditorMode = mapEditorMode;
		_plantingMapSerializer = plantingMapSerializer;
		_blockService = blockService;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(PlantingServiceKey, out var objectLoader))
		{
			_plantingMap = objectLoader.Get(PlantingMapKey, _plantingMapSerializer);
			foreach (Vector3Int item in _plantingMap.GetCoordinatesWithSetResource())
			{
				UpdatePlantingSpot(item);
			}
		}
		else
		{
			_plantingMap = new PlantingMap(_terrainService.Size);
		}
		_eventBus.Register(this);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(PlantingServiceKey).Set(PlantingMapKey, _plantingMap, _plantingMapSerializer);
		}
	}

	public void PostLoad()
	{
		foreach (Vector3Int item in _plantingMap.GetCoordinatesWithSetResource())
		{
			_terrainService.SetField(item);
		}
	}

	[OnEvent]
	public void OnBlockObjectSet(BlockObjectSetEvent blockObjectSetEvent)
	{
		UpdateOccupiedPlantingSpots(blockObjectSetEvent.BlockObject);
	}

	[OnEvent]
	public void OnBlockObjectUnset(BlockObjectUnsetEvent blockObjectUnsetEvent)
	{
		UpdateOccupiedPlantingSpots(blockObjectUnsetEvent.BlockObject);
	}

	public bool IsResourceAt(Vector3Int coordinates)
	{
		return _plantingMap.GetResource(coordinates) != null;
	}

	public string GetResourceAt(Vector3Int coordinates)
	{
		return _plantingMap.GetResource(coordinates);
	}

	public PlantingSpot? GetSpotAt(Vector3Int coordinates)
	{
		if (!_plantingSpots.TryGetValue(coordinates, out var value))
		{
			return null;
		}
		return value;
	}

	public void SetPlantingCoordinates(Vector3Int coordinates, string resource)
	{
		UnsetPlantingCoordinates(coordinates);
		_terrainService.SetField(coordinates);
		_plantingMap.SetResource(coordinates, resource);
		UpdatePlantingSpot(coordinates);
		_eventBus.Post(new PlantingCoordinatesSetEvent(coordinates, resource));
	}

	public void UnsetPlantingCoordinates(Vector3Int coordinates)
	{
		_terrainService.UnsetField(coordinates);
		string resource = _plantingMap.GetResource(coordinates);
		_plantingMap.UnsetResource(coordinates);
		_plantingSpots.Remove(coordinates);
		_eventBus.Post(new PlantingCoordinatesUnsetEvent(coordinates, resource));
	}

	public void ReservePlantingCoordinates(Vector3Int coordinates)
	{
		_reservedCoordinates.Add(coordinates);
		_plantingSpots.Remove(coordinates);
	}

	public void UnreservePlantingCoordinates(Vector3Int coordinates)
	{
		_reservedCoordinates.Remove(coordinates);
		UpdatePlantingSpot(coordinates);
	}

	public bool TryGetPlantingBlocker(Vector3Int coordinates, out BlockObject plantingBlocker)
	{
		string resourceAt = GetResourceAt(coordinates);
		if (resourceAt != null)
		{
			plantingBlocker = CreatePlantingSpot(coordinates, resourceAt).PlantingBlocker;
			return plantingBlocker != null;
		}
		plantingBlocker = null;
		return false;
	}

	private void UpdateOccupiedPlantingSpots(BlockObject blockObject)
	{
		foreach (Vector3Int occupiedCoordinate in blockObject.PositionedBlocks.GetOccupiedCoordinates())
		{
			UpdatePlantingSpotAtTerrainHeight(occupiedCoordinate);
		}
	}

	private void UpdatePlantingSpotAtTerrainHeight(Vector3Int coordinates)
	{
		UpdatePlantingSpot(coordinates);
	}

	private void UpdatePlantingSpot(Vector3Int coordinates)
	{
		string resourceAt = GetResourceAt(coordinates);
		if (resourceAt != null && !_reservedCoordinates.Contains(coordinates))
		{
			_plantingSpots[coordinates] = CreatePlantingSpot(coordinates, resourceAt);
		}
		else
		{
			_plantingSpots.Remove(coordinates);
		}
	}

	private PlantingSpot CreatePlantingSpot(Vector3Int coordinates, string resourceToPlant)
	{
		if (!_spawnValidationService.IsUnobstructed(coordinates, resourceToPlant))
		{
			BlockObject pathObjectAt = _blockService.GetPathObjectAt(coordinates);
			if (pathObjectAt != null)
			{
				return new PlantingSpot(coordinates, resourceToPlant, pathObjectAt);
			}
		}
		return new PlantingSpot(coordinates, resourceToPlant, null);
	}
}
