using System;
using System.Collections.Generic;
using Timberborn.PrioritySystem;

namespace Timberborn.WorkSystem;

public class PriorityOrderedWorkplaces
{
	private readonly struct WorkplacePriorityKey(Priority priority, int order) : IComparable<WorkplacePriorityKey>
	{
		private readonly Priority _priority = priority;

		private readonly int _order = order;

		public int CompareTo(WorkplacePriorityKey other)
		{
			int num = other._priority.CompareTo(_priority);
			if (num != 0)
			{
				return num;
			}
			return _order.CompareTo(other._order);
		}
	}

	private readonly SortedList<WorkplacePriorityKey, WorkplacePriority> _workplaces = new SortedList<WorkplacePriorityKey, WorkplacePriority>();

	public WorkplacePriority GetHighestPriorityUnderstaffedWorkplace()
	{
		for (int i = 0; i < _workplaces.Count; i++)
		{
			WorkplacePriority workplacePriority = _workplaces.Values[i];
			if (workplacePriority.Workplace.Understaffed)
			{
				return workplacePriority;
			}
		}
		return null;
	}

	public WorkplacePriority GetLowestPriorityStaffedWorkplace()
	{
		for (int num = _workplaces.Count - 1; num >= 0; num--)
		{
			WorkplacePriority workplacePriority = _workplaces.Values[num];
			if (workplacePriority.Workplace.NumberOfAssignedWorkers > 0)
			{
				return workplacePriority;
			}
		}
		return null;
	}

	public void AddWorkplace(WorkplacePriority workplacePriority)
	{
		WorkplacePriorityKey key = new WorkplacePriorityKey(workplacePriority.Priority, workplacePriority.InstantiationOrder);
		_workplaces.Add(key, workplacePriority);
		workplacePriority.PriorityChanged += OnPriorityChanged;
	}

	public void RemoveWorkplace(WorkplacePriority workplacePriority)
	{
		WorkplacePriorityKey key = new WorkplacePriorityKey(workplacePriority.Priority, workplacePriority.InstantiationOrder);
		_workplaces.Remove(key);
		workplacePriority.PriorityChanged -= OnPriorityChanged;
	}

	private void OnPriorityChanged(object sender, PriorityChangedEventArgs priorityChangedEventArgs)
	{
		WorkplacePriority workplacePriority = (WorkplacePriority)sender;
		Priority previousPriority = priorityChangedEventArgs.PreviousPriority;
		WorkplacePriorityKey key = new WorkplacePriorityKey(previousPriority, workplacePriority.InstantiationOrder);
		_workplaces.Remove(key);
		WorkplacePriorityKey key2 = new WorkplacePriorityKey(workplacePriority.Priority, workplacePriority.InstantiationOrder);
		_workplaces.Add(key2, workplacePriority);
	}
}
