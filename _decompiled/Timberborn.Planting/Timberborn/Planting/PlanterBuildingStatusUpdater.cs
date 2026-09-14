using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Timberborn.Planting;

public class PlanterBuildingStatusUpdater : BaseComponent, IAwakableComponent, IFinishedStateListener, INavMeshListener
{
	private readonly EventBus _eventBus;

	private readonly PlantingService _plantingService;

	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private NothingToDoInRangeStatus _nothingToDoInRangeStatus;

	private PlanterBuilding _planterBuilding;

	private BuildingTerrainRange _buildingTerrainRange;

	private Workplace _workplace;

	private bool _shouldUpdateStatus;

	public PlanterBuildingStatusUpdater(EventBus eventBus, PlantingService plantingService, INavMeshListenerEntityRegistry navMeshListenerEntityRegistry)
	{
		_eventBus = eventBus;
		_plantingService = plantingService;
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
	}

	public void Awake()
	{
		_nothingToDoInRangeStatus = GetComponent<NothingToDoInRangeStatus>();
		_planterBuilding = GetComponent<PlanterBuilding>();
		_buildingTerrainRange = GetComponent<BuildingTerrainRange>();
		_workplace = GetComponent<Workplace>();
		_workplace.WorkerAssigned += delegate
		{
			OnWorkerAssigned();
		};
		DisableComponent();
	}

	public void DeactivateStatus()
	{
		_nothingToDoInRangeStatus.DeactivateStatus();
	}

	public void UpdateStatus()
	{
		if (_shouldUpdateStatus)
		{
			_shouldUpdateStatus = false;
			UpdateStatusInternal();
		}
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_eventBus.Register(this);
		_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_eventBus.Unregister(this);
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_shouldUpdateStatus = true;
	}

	[OnEvent]
	public void OnPlantingAreaMarked(PlantingAreaMarkedEvent plantingAreaMarkedEvent)
	{
		UpdateStatusInternal();
	}

	[OnEvent]
	public void OnPlantingCoordinatesUnset(PlantingCoordinatesUnsetEvent plantingCoordinatesUnsetEvent)
	{
		_shouldUpdateStatus = true;
	}

	private void OnWorkerAssigned()
	{
		if (_workplace.NumberOfAssignedWorkers == 1)
		{
			_shouldUpdateStatus = true;
		}
	}

	private void UpdateStatusInternal()
	{
		if (HasValidSpot())
		{
			_nothingToDoInRangeStatus.DeactivateStatus();
		}
		else
		{
			_nothingToDoInRangeStatus.ActivateStatus();
		}
	}

	private bool HasValidSpot()
	{
		foreach (Vector3Int item in _buildingTerrainRange.GetRange())
		{
			if (IsValidSpot(item))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsValidSpot(Vector3Int coords)
	{
		string resourceAt = _plantingService.GetResourceAt(coords);
		if (resourceAt != null)
		{
			return _planterBuilding.CanPlant(resourceAt);
		}
		return false;
	}
}
