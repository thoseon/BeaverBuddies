using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.Demolishing;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Timberborn.Planting;

public class PlanterWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private readonly PlantingService _plantingService;

	private PlanterBuildingStatusUpdater _planterBuildingStatusUpdater;

	private PlantingSpotFinder _plantingSpotFinder;

	private Workplace _workplace;

	public PlanterWorkplaceBehavior(PlantingService plantingService)
	{
		_plantingService = plantingService;
	}

	public void Awake()
	{
		_planterBuildingStatusUpdater = GetComponent<PlanterBuildingStatusUpdater>();
		_plantingSpotFinder = GetComponent<PlantingSpotFinder>();
		_workplace = GetComponent<Workplace>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (agent.GetComponent<Worker>().Workplace != _workplace)
		{
			return Decision.ReleaseNow();
		}
		Demolisher component = agent.GetComponent<Demolisher>();
		if (component.HasReservedDemolishable)
		{
			return StartDemolition(agent);
		}
		Planter component2 = agent.GetComponent<Planter>();
		PlantingSpot? plantingSpot = null;
		if (!component2.PlantingCoordinates.HasValue)
		{
			plantingSpot = _plantingSpotFinder.FindClosest(agent.Transform.position);
			if (!plantingSpot.HasValue)
			{
				return Decision.ReleaseNow();
			}
		}
		Vector3Int coordinates = component2.PlantingCoordinates ?? plantingSpot.Value.Coordinates;
		if (_plantingService.TryGetPlantingBlocker(coordinates, out var plantingBlocker))
		{
			Demolishable component3 = plantingBlocker.GetComponent<Demolishable>();
			component.ReserveWithForcedDemolition(component3);
			Vector3Int coordinates2 = component3.GetComponent<BlockObject>().Coordinates;
			component2.Reserve(coordinates2);
			return StartDemolition(agent);
		}
		PlantBehavior component4 = agent.GetComponent<PlantBehavior>();
		Decision decision = component4.StartPlanting(agent);
		if (!decision.ShouldReleaseNow)
		{
			_planterBuildingStatusUpdater.DeactivateStatus();
			return Decision.TransferNow(component4, in decision);
		}
		_planterBuildingStatusUpdater.UpdateStatus();
		return Decision.ReleaseNow();
	}

	private static Decision StartDemolition(BehaviorAgent agent)
	{
		return agent.GetComponent<DemolishBehavior>().Decide(agent);
	}
}
