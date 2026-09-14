using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;

namespace Timberborn.GameDistricts;

public class DistrictConstructionAssigner : ILoadableSingleton, ISingletonInstantNavMeshListener
{
	private readonly EventBus _eventBus;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly List<DistrictBuilding> _constructions = new List<DistrictBuilding>();

	private readonly List<DistrictBuilding> _newConstructionsToAssign = new List<DistrictBuilding>();

	public DistrictConstructionAssigner(EventBus eventBus, DistrictCenterRegistry districtCenterRegistry)
	{
		_eventBus = eventBus;
		_districtCenterRegistry = districtCenterRegistry;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEnteredUnfinishedState(EnteredUnfinishedStateEvent enteredUnfinishedStateEvent)
	{
		if (enteredUnfinishedStateEvent.BlockObject.TryGetComponent<DistrictBuilding>(out var component))
		{
			RegisterConstruction(component);
		}
	}

	[OnEvent]
	public void OnExitedUnfinishedState(ExitedUnfinishedStateEvent exitedUnfinishedStateEvent)
	{
		if (exitedUnfinishedStateEvent.BlockObject.TryGetComponent<DistrictBuilding>(out var component))
		{
			UnregisterConstruction(component);
		}
	}

	[OnEvent]
	public void OnDistrictCenterRegistryChanged(DistrictCenterRegistryChangedEvent districtCenterRegistryChangedEvent)
	{
		ReassignAllConstructions();
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		if (navMeshUpdate.UpdatedRoads)
		{
			ReassignAllConstructions();
		}
		else
		{
			AssignNewConstructions();
		}
		_newConstructionsToAssign.Clear();
	}

	private void RegisterConstruction(DistrictBuilding districtBuilding)
	{
		_constructions.Add(districtBuilding);
		AssignConstructionDistrict(districtBuilding);
		if (!districtBuilding.ConstructionDistrict)
		{
			_newConstructionsToAssign.Add(districtBuilding);
		}
	}

	private void UnregisterConstruction(DistrictBuilding districtBuilding)
	{
		_constructions.Remove(districtBuilding);
		districtBuilding.UnassignConstructionDistrict();
		_newConstructionsToAssign.Remove(districtBuilding);
	}

	private void ReassignAllConstructions()
	{
		foreach (DistrictBuilding construction in _constructions)
		{
			DistrictCenter constructionDistrict = construction.ConstructionDistrict;
			if ((bool)constructionDistrict && !construction.ShouldBeAssignedToConstructionDistrict(constructionDistrict))
			{
				construction.UnassignConstructionDistrict();
			}
			if (!construction.ConstructionDistrict)
			{
				AssignConstructionDistrict(construction);
			}
		}
	}

	private void AssignNewConstructions()
	{
		if (_newConstructionsToAssign.IsEmpty())
		{
			return;
		}
		foreach (DistrictBuilding item in _newConstructionsToAssign)
		{
			AssignConstructionDistrict(item);
		}
	}

	private void AssignConstructionDistrict(DistrictBuilding building)
	{
		foreach (DistrictCenter finishedDistrictCenter in _districtCenterRegistry.FinishedDistrictCenters)
		{
			if (building.ShouldBeAssignedToConstructionDistrict(finishedDistrictCenter))
			{
				building.AssignConstructionDistrict(finishedDistrictCenter);
				return;
			}
		}
		building.UnassignConstructionDistrict();
	}
}
