using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.PopulationWorkStatistics;

public class DistrictEmploymentStatisticsProvider : BaseComponent, IAwakableComponent, IEmploymentStatisticsProvider
{
	private readonly Dictionary<string, EmploymentStatistics> _workerTypesEmploymentStatistics = new Dictionary<string, EmploymentStatistics>();

	public void Awake()
	{
		DistrictBuildingRegistry component = GetComponent<DistrictBuildingRegistry>();
		component.FinishedBuildingRegistered += OnFinishedBuildingRegistered;
		component.FinishedBuildingUnregistered += OnFinishedBuildingUnregistered;
	}

	public EmploymentStatistics GetEmploymentStatistics(string workerType)
	{
		if (!_workerTypesEmploymentStatistics.TryGetValue(workerType, out var value))
		{
			return new EmploymentStatistics(0, 0, workerType);
		}
		return value;
	}

	private void OnFinishedBuildingRegistered(object sender, FinishedBuildingRegisteredEventArgs finishedBuildingRegisteredEventArgs)
	{
		WorkplaceWorkerCounter component = finishedBuildingRegisteredEventArgs.Building.GetComponent<WorkplaceWorkerCounter>();
		if (component != null)
		{
			AddWorkplaceWorkerCounter(component);
		}
	}

	private void OnFinishedBuildingUnregistered(object sender, FinishedBuildingUnregisteredEventArgs finishedBuildingUnregisteredEventArgs)
	{
		WorkplaceWorkerCounter component = finishedBuildingUnregisteredEventArgs.Building.GetComponent<WorkplaceWorkerCounter>();
		if (component != null)
		{
			RemoveWorkplaceWorkerCounter(component);
		}
	}

	private void AddWorkplaceWorkerCounter(WorkplaceWorkerCounter workplaceWorkerCounter)
	{
		EmploymentStatistics currentEmploymentStatistics = workplaceWorkerCounter.GetCurrentEmploymentStatistics();
		AddWorkerTypeIfNeeded(currentEmploymentStatistics.WorkerType);
		_workerTypesEmploymentStatistics[currentEmploymentStatistics.WorkerType] += currentEmploymentStatistics;
		workplaceWorkerCounter.WorkerCountChanged += OnWorkerCountChanged;
	}

	private void RemoveWorkplaceWorkerCounter(WorkplaceWorkerCounter workplaceWorkerCounter)
	{
		EmploymentStatistics currentEmploymentStatistics = workplaceWorkerCounter.GetCurrentEmploymentStatistics();
		_workerTypesEmploymentStatistics[currentEmploymentStatistics.WorkerType] -= currentEmploymentStatistics;
		workplaceWorkerCounter.WorkerCountChanged -= OnWorkerCountChanged;
	}

	private void OnWorkerCountChanged(object sender, WorkerCountChangedEventArgs workerCountChangedEventArgs)
	{
		EmploymentStatistics oldEmploymentStatistics = workerCountChangedEventArgs.OldEmploymentStatistics;
		_workerTypesEmploymentStatistics[oldEmploymentStatistics.WorkerType] -= oldEmploymentStatistics;
		EmploymentStatistics newEmploymentStatistics = workerCountChangedEventArgs.NewEmploymentStatistics;
		AddWorkerTypeIfNeeded(newEmploymentStatistics.WorkerType);
		_workerTypesEmploymentStatistics[newEmploymentStatistics.WorkerType] += newEmploymentStatistics;
	}

	private void AddWorkerTypeIfNeeded(string workerType)
	{
		_workerTypesEmploymentStatistics.TryAdd(workerType, new EmploymentStatistics(0, 0, workerType));
	}
}
