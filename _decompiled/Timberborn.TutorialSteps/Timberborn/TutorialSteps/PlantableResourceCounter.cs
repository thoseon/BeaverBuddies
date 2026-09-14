using System.Collections.Generic;
using Timberborn.Planting;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TutorialSteps;

public class PlantableResourceCounter : ILoadableSingleton
{
	private readonly PlantingService _plantingService;

	private readonly EventBus _eventBus;

	private readonly Dictionary<string, int> _resources = new Dictionary<string, int>();

	public PlantableResourceCounter(PlantingService plantingService, EventBus eventBus)
	{
		_plantingService = plantingService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		foreach (Vector3Int plantingCoordinate in _plantingService.PlantingCoordinates)
		{
			string resourceAt = _plantingService.GetResourceAt(plantingCoordinate);
			if (resourceAt != null)
			{
				ModifyNumberOfResources(resourceAt, 1);
			}
		}
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnPlantingCoordinatesSet(PlantingCoordinatesSetEvent plantingCoordinatesSetEvent)
	{
		ModifyNumberOfResources(plantingCoordinatesSetEvent.Resource, 1);
	}

	[OnEvent]
	public void OnPlantingCoordinatesUnset(PlantingCoordinatesUnsetEvent plantingCoordinatesUnsetEvent)
	{
		if (plantingCoordinatesUnsetEvent.Resource != null)
		{
			ModifyNumberOfResources(plantingCoordinatesUnsetEvent.Resource, -1);
		}
	}

	public int GetNumberOfResources(string resource)
	{
		if (!_resources.TryGetValue(resource, out var value))
		{
			return 0;
		}
		return value;
	}

	private void ModifyNumberOfResources(string resource, int change)
	{
		_resources[resource] = GetNumberOfResources(resource) + change;
	}
}
