using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Planting;

public class Planter : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity
{
	private static readonly ComponentKey PlanterKey = new ComponentKey("Planter");

	private static readonly PropertyKey<Vector3Int> PlantingCoordinatesKey = new PropertyKey<Vector3Int>("PlantingCoordinates");

	private readonly PlantingService _plantingService;

	public Vector3Int? PlantingCoordinates { get; private set; }

	public Planter(PlantingService plantingService)
	{
		_plantingService = plantingService;
	}

	public void Awake()
	{
		GetComponent<Worker>().GotUnemployed += delegate
		{
			Unreserve();
		};
	}

	public void DeleteEntity()
	{
		Unreserve();
	}

	public void Reserve(Vector3Int plantingCoordinates)
	{
		PlantingCoordinates = plantingCoordinates;
		_plantingService.ReservePlantingCoordinates(plantingCoordinates);
	}

	public void Unreserve()
	{
		if (PlantingCoordinates.HasValue)
		{
			_plantingService.UnreservePlantingCoordinates(PlantingCoordinates.Value);
			PlantingCoordinates = null;
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (PlantingCoordinates.HasValue)
		{
			entitySaver.GetComponent(PlanterKey).Set(PlantingCoordinatesKey, PlantingCoordinates.Value);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(PlanterKey, out var objectLoader))
		{
			Reserve(objectLoader.Get(PlantingCoordinatesKey));
		}
	}
}
