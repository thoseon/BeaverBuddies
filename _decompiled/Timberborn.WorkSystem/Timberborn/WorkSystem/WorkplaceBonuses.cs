using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;

namespace Timberborn.WorkSystem;

public class WorkplaceBonuses : BaseComponent, IAwakableComponent
{
	private WorkplaceBonusesSpec _workplaceBonusesSpec;

	public ImmutableArray<BonusSpec> WorkerBonuses => _workplaceBonusesSpec.WorkerBonuses;

	public void Awake()
	{
		_workplaceBonusesSpec = GetComponent<WorkplaceBonusesSpec>();
		Workplace component = GetComponent<Workplace>();
		component.WorkerAssigned += OnWorkerAssigned;
		component.WorkerUnassigned += OnWorkerUnassigned;
	}

	private void OnWorkerAssigned(object sender, WorkerChangedEventArgs e)
	{
		AddBonuses(e.Worker);
	}

	private void OnWorkerUnassigned(object sender, WorkerChangedEventArgs e)
	{
		RemoveBonuses(e.Worker);
	}

	private void AddBonuses(Worker worker)
	{
		worker.GetComponent<BonusManager>().AddBonuses(WorkerBonuses);
	}

	private void RemoveBonuses(Worker worker)
	{
		worker.GetComponent<BonusManager>().RemoveBonuses(WorkerBonuses);
	}
}
