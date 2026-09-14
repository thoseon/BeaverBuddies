using System.Collections.Generic;
using System.Collections.ObjectModel;
using Timberborn.PrioritySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.ConstructionSites;

public class ConstructionRegistry : ILoadableSingleton
{
	private readonly Dictionary<Priority, SortedList<int, ConstructionJob>> _jobs = new Dictionary<Priority, SortedList<int, ConstructionJob>>();

	private readonly Dictionary<Priority, ReadOnlyCollection<ConstructionJob>> _jobsAsReadOnly = new Dictionary<Priority, ReadOnlyCollection<ConstructionJob>>();

	public ReadOnlyCollection<ConstructionJob> GetJobs(Priority priority)
	{
		return _jobsAsReadOnly[priority];
	}

	public void Load()
	{
		foreach (Priority item in Priorities.Ascending)
		{
			SortedList<int, ConstructionJob> sortedList = new SortedList<int, ConstructionJob>();
			_jobs[item] = sortedList;
			_jobsAsReadOnly[item] = new ReadOnlyCollection<ConstructionJob>(sortedList.Values);
		}
	}

	public void AddJob(ConstructionJob job, Priority priority, int order)
	{
		_jobs[priority].Add(order, job);
	}

	public void RemoveJob(Priority priority, int order)
	{
		_jobs[priority].Remove(order);
	}
}
