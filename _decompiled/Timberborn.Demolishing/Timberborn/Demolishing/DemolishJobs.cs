using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.PrioritySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Demolishing;

internal class DemolishJobs : ILoadableSingleton
{
	private readonly Dictionary<Priority, List<DemolishJob>> _jobs = new Dictionary<Priority, List<DemolishJob>>();

	public ReadOnlyList<DemolishJob> GetJobs(Priority priority)
	{
		return _jobs[priority].AsReadOnlyList();
	}

	public void Load()
	{
		foreach (Priority item in Priorities.Ascending)
		{
			_jobs[item] = new List<DemolishJob>();
		}
	}

	public void AddJob(DemolishJob job, Priority priority)
	{
		_jobs[priority].Add(job);
	}

	public void RemoveJob(DemolishJob job, Priority priority)
	{
		_jobs[priority].Remove(job);
	}
}
