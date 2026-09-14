using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;

namespace Timberborn.GameDistricts;

internal class DistrictBuildingAssigner : ILoadableSingleton, ISingletonNavMeshListener, ISingletonInstantNavMeshListener
{
	private readonly EventBus _eventBus;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly List<DistrictBuilding> _allBuildings = new List<DistrictBuilding>();

	private readonly List<DistrictBuilding> _newBuildingsToAssign = new List<DistrictBuilding>();

	private readonly List<DistrictBuilding> _newInstantBuildingsToAssign = new List<DistrictBuilding>();

	public DistrictBuildingAssigner(EventBus eventBus, DistrictCenterRegistry districtCenterRegistry)
	{
		_eventBus = eventBus;
		_districtCenterRegistry = districtCenterRegistry;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (enteredFinishedStateEvent.BlockObject.TryGetComponent<DistrictBuilding>(out var component))
		{
			RegisterBuilding(component);
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		if (exitedFinishedStateEvent.BlockObject.TryGetComponent<DistrictBuilding>(out var component))
		{
			UnregisterBuilding(component);
		}
	}

	[OnEvent]
	public void OnDistrictCenterRegistryChanged(DistrictCenterRegistryChangedEvent districtCenterRegistryChangedEvent)
	{
		ReassignAllBuildings();
		ReassignAllInstantBuildings();
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		if (navMeshUpdate.UpdatedRoads)
		{
			ReassignAllBuildings();
		}
		else
		{
			AssignNewBuildings();
		}
		_newBuildingsToAssign.Clear();
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		if (navMeshUpdate.UpdatedRoads)
		{
			ReassignAllInstantBuildings();
		}
		else
		{
			AssignNewInstantBuildings();
		}
		_newInstantBuildingsToAssign.Clear();
	}

	private void RegisterBuilding(DistrictBuilding districtBuilding)
	{
		_allBuildings.Add(districtBuilding);
		AssignDistrict(districtBuilding);
		AssignInstantDistrict(districtBuilding);
		if (!districtBuilding.District)
		{
			_newBuildingsToAssign.Add(districtBuilding);
		}
		if (!districtBuilding.InstantDistrict)
		{
			_newInstantBuildingsToAssign.Add(districtBuilding);
		}
	}

	private void UnregisterBuilding(DistrictBuilding districtBuilding)
	{
		_allBuildings.Remove(districtBuilding);
		EntityComponent component = districtBuilding.GetComponent<EntityComponent>();
		DistrictCenter district = districtBuilding.District;
		if ((bool)district)
		{
			district.DistrictBuildingRegistry.UnregisterFinishedBuilding(component);
		}
		districtBuilding.UnassignDistrict();
		DistrictCenter instantDistrict = districtBuilding.InstantDistrict;
		if ((bool)instantDistrict)
		{
			instantDistrict.DistrictBuildingRegistry.UnregisterInstantFinishedBuilding(component);
		}
		districtBuilding.UnassignInstantDistrict();
		_newBuildingsToAssign.Remove(districtBuilding);
		_newInstantBuildingsToAssign.Remove(districtBuilding);
	}

	private void ReassignAllBuildings()
	{
		foreach (DistrictBuilding allBuilding in _allBuildings)
		{
			DistrictCenter district = allBuilding.District;
			if ((bool)district && !allBuilding.ShouldBeAssignedToDistrict(district))
			{
				allBuilding.UnassignDistrict();
				district.DistrictBuildingRegistry.UnregisterFinishedBuilding(allBuilding.GetComponent<EntityComponent>());
			}
			if (!allBuilding.District)
			{
				AssignDistrict(allBuilding);
			}
		}
	}

	private void AssignNewBuildings()
	{
		if (_newBuildingsToAssign.IsEmpty())
		{
			return;
		}
		foreach (DistrictBuilding item in _newBuildingsToAssign)
		{
			AssignDistrict(item);
		}
	}

	private void AssignDistrict(DistrictBuilding building)
	{
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			if (building.ShouldBeAssignedToDistrict(finishedDistrictCenter))
			{
				building.AssignDistrict(finishedDistrictCenter);
				finishedDistrictCenter.DistrictBuildingRegistry.RegisterFinishedBuilding(building.GetComponent<EntityComponent>());
				return;
			}
		}
		building.UnassignDistrict();
	}

	private void ReassignAllInstantBuildings()
	{
		foreach (DistrictBuilding allBuilding in _allBuildings)
		{
			DistrictCenter instantDistrict = allBuilding.InstantDistrict;
			if ((bool)instantDistrict && !allBuilding.ShouldBeAssignedToInstantDistrict(instantDistrict))
			{
				instantDistrict.DistrictBuildingRegistry.UnregisterInstantFinishedBuilding(allBuilding.GetComponent<EntityComponent>());
				allBuilding.UnassignInstantDistrict();
			}
			if (!allBuilding.InstantDistrict)
			{
				AssignInstantDistrict(allBuilding);
			}
		}
	}

	private void AssignNewInstantBuildings()
	{
		if (_newInstantBuildingsToAssign.IsEmpty())
		{
			return;
		}
		foreach (DistrictBuilding item in _newInstantBuildingsToAssign)
		{
			AssignInstantDistrict(item);
		}
	}

	private void AssignInstantDistrict(DistrictBuilding building)
	{
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			if (building.ShouldBeAssignedToInstantDistrict(finishedDistrictCenter))
			{
				building.AssignInstantDistrict(finishedDistrictCenter);
				finishedDistrictCenter.DistrictBuildingRegistry.RegisterInstantFinishedBuilding(building.GetComponent<EntityComponent>());
				return;
			}
		}
		building.UnassignInstantDistrict();
	}
}
