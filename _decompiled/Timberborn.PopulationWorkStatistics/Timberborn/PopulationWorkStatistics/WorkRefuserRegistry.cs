using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.WorkSystem;

namespace Timberborn.PopulationWorkStatistics;

internal class WorkRefuserRegistry
{
	private class WorkRefuserAssigner
	{
		private readonly HashSet<WorkRefuser> _refusingWorkRefusers = new HashSet<WorkRefuser>();

		private readonly HashSet<WorkRefuser> _notRefusingWorkRefusers = new HashSet<WorkRefuser>();

		public int NumberOfRefusingWorkers => _refusingWorkRefusers.Count;

		public int NumberOfNotRefusingWorkers => _notRefusingWorkRefusers.Count;

		public void ReassignWorkRefuser(WorkRefuser workRefuser)
		{
			_refusingWorkRefusers.Remove(workRefuser);
			_notRefusingWorkRefusers.Remove(workRefuser);
			if (workRefuser.RefusesWork)
			{
				_refusingWorkRefusers.Add(workRefuser);
			}
			else
			{
				_notRefusingWorkRefusers.Add(workRefuser);
			}
		}

		public void RemoveWorkRefuser(WorkRefuser workRefuser)
		{
			_refusingWorkRefusers.Remove(workRefuser);
			_notRefusingWorkRefusers.Remove(workRefuser);
		}
	}

	private readonly Dictionary<string, WorkRefuserAssigner> _workerTypeWorkRefusers = new Dictionary<string, WorkRefuserAssigner>();

	public WorkRefusingStatistics GetWorkRefusingStatistics(string workerType)
	{
		if (_workerTypeWorkRefusers.TryGetValue(workerType, out var value))
		{
			return new WorkRefusingStatistics(value.NumberOfRefusingWorkers, value.NumberOfNotRefusingWorkers);
		}
		return new WorkRefusingStatistics(0, 0);
	}

	public void AddWorkRefuser(WorkRefuser workRefuser)
	{
		GetWorkRefuserAssigner(workRefuser).ReassignWorkRefuser(workRefuser);
		workRefuser.RefusesWorkChanged += OnRefusesWorkChanged;
	}

	public void RemoveWorkRefuser(WorkRefuser workRefuser)
	{
		GetWorkRefuserAssigner(workRefuser).RemoveWorkRefuser(workRefuser);
		workRefuser.RefusesWorkChanged -= OnRefusesWorkChanged;
	}

	private WorkRefuserAssigner GetWorkRefuserAssigner(WorkRefuser workRefuser)
	{
		string workerType = workRefuser.GetComponent<Worker>().WorkerType;
		return _workerTypeWorkRefusers.GetOrAdd(workerType);
	}

	private void OnRefusesWorkChanged(object sender, EventArgs e)
	{
		WorkRefuser workRefuser = (WorkRefuser)sender;
		GetWorkRefuserAssigner(workRefuser).ReassignWorkRefuser(workRefuser);
	}
}
