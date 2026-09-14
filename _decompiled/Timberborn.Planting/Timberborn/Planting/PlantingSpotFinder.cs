using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.NaturalResources;
using Timberborn.NaturalResourcesMoisture;
using UnityEngine;

namespace Timberborn.Planting;

internal class PlantingSpotFinder : BaseComponent, IAwakableComponent
{
	private readonly PlantingService _plantingService;

	private readonly FloodableNaturalResourceService _floodableNaturalResourceService;

	private readonly SpawnValidationService _spawnValidationService;

	private readonly PlantingSoilValidator _plantingSoilValidator;

	private PlantablePrioritizer _plantablePrioritizer;

	private BlockObjectCenter _blockObjectCenter;

	private IPlantingSpotValidator _plantingSpotValidator;

	private InRangePlantingCoordinates _inRangePlantingCoordinates;

	public PlantingSpotFinder(PlantingService plantingService, FloodableNaturalResourceService floodableNaturalResourceService, SpawnValidationService spawnValidationService, PlantingSoilValidator plantingSoilValidator)
	{
		_plantingService = plantingService;
		_floodableNaturalResourceService = floodableNaturalResourceService;
		_spawnValidationService = spawnValidationService;
		_plantingSoilValidator = plantingSoilValidator;
	}

	public void Awake()
	{
		_plantablePrioritizer = GetComponent<PlantablePrioritizer>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_plantingSpotValidator = GetComponent<IPlantingSpotValidator>();
		_inRangePlantingCoordinates = GetComponent<InRangePlantingCoordinates>();
	}

	public PlantingSpot? FindClosest(Vector3 agentPosition)
	{
		PlantableSpec prioritizedPlantableSpec = _plantablePrioritizer.PrioritizedPlantableSpec;
		if (prioritizedPlantableSpec != null)
		{
			PlantingSpot? plantingSpot = FindClosest(agentPosition, prioritizedPlantableSpec);
			if (plantingSpot.HasValue)
			{
				return plantingSpot.GetValueOrDefault();
			}
		}
		return FindClosest(agentPosition, null);
	}

	private PlantingSpot? FindClosest(Vector3 agentPosition, PlantableSpec prioritizedPlantableSpec)
	{
		return GetClosestOrDefault(GetNeighboring(agentPosition), prioritizedPlantableSpec) ?? GetClosestOrDefault(GetReachable(), prioritizedPlantableSpec);
	}

	private PlantingSpot? GetClosestOrDefault(IEnumerable<PlantingSpot> plantingCoordinates, PlantableSpec prioritizedPlantableSpec)
	{
		float num = float.PositiveInfinity;
		PlantingSpot? result = null;
		foreach (PlantingSpot plantingCoordinate in plantingCoordinates)
		{
			float num2 = Vector3.Distance(_blockObjectCenter.WorldCenterGrounded, CoordinateSystem.GridToWorldCentered(plantingCoordinate.Coordinates));
			if (num2 < num && CanPlantAt(plantingCoordinate, prioritizedPlantableSpec))
			{
				result = plantingCoordinate;
				num = num2;
			}
		}
		return result;
	}

	private IEnumerable<PlantingSpot> GetNeighboring(Vector3 agentPosition)
	{
		Vector3Int agentCoordinates = CoordinateSystem.WorldToGridInt(agentPosition);
		Vector3Int[] neighbors8Vector3IntOrdered = Deltas.Neighbors8Vector3IntOrdered;
		foreach (Vector3Int vector3Int in neighbors8Vector3IntOrdered)
		{
			Vector3Int coordinates = agentCoordinates + vector3Int;
			if (_inRangePlantingCoordinates.AreCoordinatesInRange(coordinates))
			{
				PlantingSpot? spotAt = _plantingService.GetSpotAt(coordinates);
				if (spotAt.HasValue)
				{
					yield return spotAt.Value;
				}
			}
		}
	}

	private IEnumerable<PlantingSpot> GetReachable()
	{
		foreach (Vector3Int coordinate in _inRangePlantingCoordinates.GetCoordinates())
		{
			PlantingSpot? spotAt = _plantingService.GetSpotAt(coordinate);
			if (spotAt.HasValue)
			{
				yield return spotAt.Value;
			}
		}
	}

	private bool CanPlantAt(PlantingSpot plantingSpot, PlantableSpec prioritizedPlantableSpec)
	{
		string resourceToPlant = plantingSpot.ResourceToPlant;
		if (prioritizedPlantableSpec != null && resourceToPlant != prioritizedPlantableSpec.TemplateName)
		{
			return false;
		}
		if (!_plantingSoilValidator.Validate(plantingSpot))
		{
			return false;
		}
		if (!_plantingSpotValidator.Validate(plantingSpot))
		{
			return false;
		}
		Vector3Int coordinates = plantingSpot.Coordinates;
		if (!_floodableNaturalResourceService.ConditionsAreMet(resourceToPlant, coordinates))
		{
			return false;
		}
		if (!plantingSpot.PlantingBlocker)
		{
			return _spawnValidationService.IsUnobstructed(coordinates, resourceToPlant);
		}
		return true;
	}
}
