using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Coordinates;
using Timberborn.WalkingSystem;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Timberborn.Planting;

public class PlantBehavior : Behavior, IAwakableComponent, IJobBehavior
{
	private readonly PlantingService _plantingService;

	private Planter _planter;

	private Worker _worker;

	private WalkToPositionExecutor _walkToPositionExecutor;

	private PlantExecutor _plantExecutor;

	public PlantBehavior(PlantingService plantingService)
	{
		_plantingService = plantingService;
	}

	public void Awake()
	{
		_planter = GetComponent<Planter>();
		_worker = GetComponent<Worker>();
		_walkToPositionExecutor = GetComponent<WalkToPositionExecutor>();
		_plantExecutor = GetComponent<PlantExecutor>();
	}

	public Decision StartPlanting(BehaviorAgent agent)
	{
		ReserveCoordinates(agent);
		return Decide(agent);
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_planter.PlantingCoordinates.HasValue)
		{
			Vector3 position = CoordinateSystem.GridToWorldCentered(_planter.PlantingCoordinates.Value);
			switch (_walkToPositionExecutor.Launch(position))
			{
			case ExecutorStatus.Success:
			{
				Vector3Int? plantingCoordinates = _planter.PlantingCoordinates;
				_planter.Unreserve();
				return Plant(plantingCoordinates.Value);
			}
			case ExecutorStatus.Failure:
				_planter.Unreserve();
				return Decision.ReleaseNextTick();
			case ExecutorStatus.Running:
				return Decision.ReturnWhenFinished(_walkToPositionExecutor);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Decision.ReleaseNow();
	}

	private void ReserveCoordinates(BehaviorAgent agent)
	{
		if (!_planter.PlantingCoordinates.HasValue)
		{
			Vector3 position = agent.Transform.position;
			PlantingSpot? plantingSpot = _worker.Workplace.GetComponent<PlantingSpotFinder>().FindClosest(position);
			if (plantingSpot.HasValue)
			{
				_planter.Reserve(plantingSpot.Value.Coordinates);
			}
		}
	}

	private Decision Plant(Vector3Int coordinates)
	{
		if (!_plantExecutor.Launch(coordinates, _plantingService.GetResourceAt(coordinates)))
		{
			return Decision.ReleaseNextTick();
		}
		return Decision.ReleaseWhenFinished(_plantExecutor);
	}
}
