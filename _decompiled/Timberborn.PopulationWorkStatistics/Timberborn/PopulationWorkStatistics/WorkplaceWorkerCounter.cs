using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.WorkSystem;

namespace Timberborn.PopulationWorkStatistics;

internal class WorkplaceWorkerCounter : BaseComponent, IAwakableComponent
{
	private BlockableObject _blockableObject;

	private Workplace _workplace;

	private WorkplaceWorkerType _workplaceWorkerType;

	private EmploymentStatistics? _currentEmploymentStatistics;

	public event EventHandler<WorkerCountChangedEventArgs> WorkerCountChanged;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_workplace = GetComponent<Workplace>();
		_workplaceWorkerType = GetComponent<WorkplaceWorkerType>();
		_blockableObject.ObjectBlocked += delegate
		{
			InvokeWorkerCountChanged();
		};
		_blockableObject.ObjectUnblocked += delegate
		{
			InvokeWorkerCountChanged();
		};
		_workplace.WorkerAssigned += delegate
		{
			InvokeWorkerCountChanged();
		};
		_workplace.WorkerUnassigned += delegate
		{
			InvokeWorkerCountChanged();
		};
		_workplace.DesiredWorkersChanged += delegate
		{
			InvokeWorkerCountChanged();
		};
		_workplaceWorkerType.WorkerTypeChanged += delegate
		{
			InvokeWorkerCountChanged();
		};
	}

	public EmploymentStatistics GetCurrentEmploymentStatistics()
	{
		EmploymentStatistics valueOrDefault = _currentEmploymentStatistics.GetValueOrDefault();
		if (!_currentEmploymentStatistics.HasValue)
		{
			valueOrDefault = GetEmploymentStatistics();
			_currentEmploymentStatistics = valueOrDefault;
		}
		return _currentEmploymentStatistics.Value;
	}

	private void InvokeWorkerCountChanged()
	{
		EmploymentStatistics employmentStatistics = GetEmploymentStatistics();
		WorkerCountChanged?.Invoke(this, new WorkerCountChangedEventArgs(GetCurrentEmploymentStatistics(), employmentStatistics));
		_currentEmploymentStatistics = employmentStatistics;
	}

	private EmploymentStatistics GetEmploymentStatistics()
	{
		if (_blockableObject.IsUnblocked)
		{
			int numberOfAssignedWorkers = _workplace.NumberOfAssignedWorkers;
			int vacancies = Math.Max(_workplace.DesiredWorkers - numberOfAssignedWorkers, 0);
			return new EmploymentStatistics(numberOfAssignedWorkers, vacancies, _workplaceWorkerType.WorkerType);
		}
		return new EmploymentStatistics(0, 0, _workplaceWorkerType.WorkerType);
	}
}
