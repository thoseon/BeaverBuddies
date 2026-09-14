using Timberborn.Common;

namespace Timberborn.WorkSystem;

internal class WorkplaceAssigner
{
	private readonly PriorityOrderedWorkplaces _priorityOrderedWorkplaces = new PriorityOrderedWorkplaces();

	private readonly UnemployedWorkers _unemployedWorkers = new UnemployedWorkers();

	public void Assign()
	{
		WorkplacePriority highestPriorityUnderstaffedWorkplace = _priorityOrderedWorkplaces.GetHighestPriorityUnderstaffedWorkplace();
		if ((bool)highestPriorityUnderstaffedWorkplace)
		{
			if (_unemployedWorkers.AnyUnemployed)
			{
				AssignStalestUnemployed(highestPriorityUnderstaffedWorkplace.Workplace);
			}
			else
			{
				ReassignWorkersToHigherPriorityWorkplaces(highestPriorityUnderstaffedWorkplace);
			}
		}
	}

	public void AddWorker(Worker worker)
	{
		_unemployedWorkers.AddWorker(worker);
	}

	public void RemoveWorker(Worker worker)
	{
		_unemployedWorkers.RemoveWorker(worker);
	}

	public void AddWorkplace(WorkplacePriority workplacePriority)
	{
		_priorityOrderedWorkplaces.AddWorkplace(workplacePriority);
	}

	public void RemoveWorkplace(WorkplacePriority workplacePriority)
	{
		_priorityOrderedWorkplaces.RemoveWorkplace(workplacePriority);
	}

	private void AssignStalestUnemployed(Workplace workplace)
	{
		int num = workplace.DesiredWorkers - workplace.NumberOfAssignedWorkers;
		while (num > 0 && _unemployedWorkers.AnyUnemployed)
		{
			workplace.AssignWorker(_unemployedWorkers.GetUnemployedWorker());
			num--;
		}
	}

	private void ReassignWorkersToHigherPriorityWorkplaces(WorkplacePriority understaffedWorkplace)
	{
		WorkplacePriority lowestPriorityStaffedWorkplace = _priorityOrderedWorkplaces.GetLowestPriorityStaffedWorkplace();
		if ((bool)lowestPriorityStaffedWorkplace && understaffedWorkplace.Priority > lowestPriorityStaffedWorkplace.Priority)
		{
			ReassignWorkers(lowestPriorityStaffedWorkplace.Workplace, understaffedWorkplace.Workplace);
		}
	}

	private static void ReassignWorkers(Workplace staffedWorkplace, Workplace understaffedWorkplace)
	{
		ReadOnlyList<Worker> assignedWorkers = staffedWorkplace.AssignedWorkers;
		int num = assignedWorkers.Count - 1;
		while (num >= 0)
		{
			Worker worker = assignedWorkers[num];
			staffedWorkplace.UnassignWorker(worker);
			understaffedWorkplace.AssignWorker(worker);
			if (understaffedWorkplace.Understaffed)
			{
				num--;
				continue;
			}
			break;
		}
	}
}
