using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Planting;

internal class InRangePlantingCoordinates : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly EventBus _eventBus;

	private readonly PlantingService _plantingService;

	private BuildingTerrainRange _buildingTerrainRange;

	private PlanterBuilding _planterBuilding;

	private readonly HashSet<Vector3Int> _coordinatesInRange = new HashSet<Vector3Int>();

	private bool _dirty;

	public InRangePlantingCoordinates(EventBus eventBus, PlantingService plantingService)
	{
		_eventBus = eventBus;
		_plantingService = plantingService;
	}

	public void Awake()
	{
		_buildingTerrainRange = GetComponent<BuildingTerrainRange>();
		_planterBuilding = GetComponent<PlanterBuilding>();
	}

	public void OnEnterFinishedState()
	{
		_buildingTerrainRange.RangeChanged += OnRangeChanged;
		_eventBus.Register(this);
		_dirty = true;
	}

	public void OnExitFinishedState()
	{
		_buildingTerrainRange.RangeChanged -= OnRangeChanged;
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnPlantingCoordinatesSet(PlantingCoordinatesSetEvent plantingCoordinatesSetEvent)
	{
		Vector3Int coordinates = plantingCoordinatesSetEvent.Coordinates;
		string resource = plantingCoordinatesSetEvent.Resource;
		if (_buildingTerrainRange.GetRange().Contains(coordinates) && IsAllowed(resource))
		{
			_coordinatesInRange.Add(coordinates);
		}
	}

	[OnEvent]
	public void OnPlantingCoordinatesUnset(PlantingCoordinatesUnsetEvent plantingCoordinatesUnsetEvent)
	{
		_coordinatesInRange.Remove(plantingCoordinatesUnsetEvent.Coordinates);
	}

	public ReadOnlyHashSet<Vector3Int> GetCoordinates()
	{
		UpdateCoordinates();
		return _coordinatesInRange.AsReadOnlyHashSet();
	}

	public bool AreCoordinatesInRange(Vector3Int coordinates)
	{
		UpdateCoordinates();
		return _coordinatesInRange.Contains(coordinates);
	}

	private void OnRangeChanged(object sender, RangeChangedEventArgs rangeChangedEventArgs)
	{
		_dirty = true;
	}

	private void UpdateCoordinates()
	{
		if (!_dirty)
		{
			return;
		}
		_coordinatesInRange.Clear();
		foreach (Vector3Int item in _buildingTerrainRange.GetRange())
		{
			PlantingSpot? spotAt = _plantingService.GetSpotAt(item);
			if (spotAt.HasValue && IsAllowed(spotAt.GetValueOrDefault().ResourceToPlant))
			{
				_coordinatesInRange.Add(item);
			}
		}
		_dirty = false;
	}

	private bool IsAllowed(string resource)
	{
		return _planterBuilding.CanPlant(resource);
	}
}
