using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.GameDistricts;
using Timberborn.TickSystem;

namespace Timberborn.WorkSystem;

internal class DistrictWorkplaceAssigner : TickableComponent, IAwakableComponent
{
	private readonly Dictionary<string, WorkplaceAssigner> _workplaceAssigners = new Dictionary<string, WorkplaceAssigner>();

	public void Awake()
	{
		DistrictBuildingRegistry component = GetComponent<DistrictBuildingRegistry>();
		component.FinishedBuildingRegistered += OnFinishedBuildingRegistered;
		component.FinishedBuildingUnregistered += OnFinishedBuildingUnregistered;
		DistrictPopulation component2 = GetComponent<DistrictPopulation>();
		component2.CitizenAssigned += OnCitizenAssigned;
		component2.CitizenUnassigned += OnCitizenUnassigned;
	}

	public override void Tick()
	{
		foreach (WorkplaceAssigner value in _workplaceAssigners.Values)
		{
			value.Assign();
		}
	}

	private void OnFinishedBuildingRegistered(object sender, FinishedBuildingRegisteredEventArgs finishedBuildingRegisteredEventArgs)
	{
		WorkplacePriority component = finishedBuildingRegisteredEventArgs.Building.GetComponent<WorkplacePriority>();
		if (component != null)
		{
			WorkplaceWorkerType component2 = component.GetComponent<WorkplaceWorkerType>();
			_workplaceAssigners.GetOrAdd(component2.WorkerType).AddWorkplace(component);
			component2.WorkerTypeChanged += OnWorkerTypeChanged;
		}
	}

	private void OnFinishedBuildingUnregistered(object sender, FinishedBuildingUnregisteredEventArgs finishedBuildingUnregisteredEventArgs)
	{
		WorkplacePriority component = finishedBuildingUnregisteredEventArgs.Building.GetComponent<WorkplacePriority>();
		if (component != null)
		{
			WorkplaceWorkerType component2 = component.GetComponent<WorkplaceWorkerType>();
			_workplaceAssigners[component2.WorkerType].RemoveWorkplace(component);
			component2.WorkerTypeChanged -= OnWorkerTypeChanged;
		}
	}

	private void OnCitizenAssigned(object sender, CitizenAssignedEventArgs citizenAssignedEventArgs)
	{
		Worker component = citizenAssignedEventArgs.Citizen.GetComponent<Worker>();
		if (component != null)
		{
			_workplaceAssigners.GetOrAdd(component.WorkerType).AddWorker(component);
		}
	}

	private void OnCitizenUnassigned(object sender, CitizenUnassignedEventArgs citizenUnassignedEventArgs)
	{
		Worker component = citizenUnassignedEventArgs.Citizen.GetComponent<Worker>();
		if (component != null)
		{
			_workplaceAssigners[component.WorkerType].RemoveWorker(component);
		}
	}

	private void OnWorkerTypeChanged(object sender, WorkerTypeChangedEventArgs workerTypeChangedEventArgs)
	{
		WorkplacePriority component = ((WorkplaceWorkerType)sender).GetComponent<WorkplacePriority>();
		string previousWorkerType = workerTypeChangedEventArgs.PreviousWorkerType;
		string currentWorkerType = workerTypeChangedEventArgs.CurrentWorkerType;
		_workplaceAssigners[previousWorkerType].RemoveWorkplace(component);
		_workplaceAssigners.GetOrAdd(currentWorkerType).AddWorkplace(component);
	}
}
