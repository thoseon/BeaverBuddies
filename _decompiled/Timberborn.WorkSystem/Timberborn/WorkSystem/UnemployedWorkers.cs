using System;
using System.Collections.Generic;
using System.Linq;

namespace Timberborn.WorkSystem;

internal class UnemployedWorkers
{
	private readonly HashSet<Worker> _unemployed = new HashSet<Worker>();

	public bool AnyUnemployed => _unemployed.Count > 0;

	public Worker GetUnemployedWorker()
	{
		return _unemployed.First();
	}

	public void AddWorker(Worker worker)
	{
		WorkRefuser component = worker.GetComponent<WorkRefuser>();
		UpdateUnemployedState(worker, component);
		worker.GotUnemployed += OnGotUnemployed;
		worker.GotEmployed += OnGotEmployed;
		component.RefusesWorkChanged += OnRefusesWorkChanged;
	}

	public void RemoveWorker(Worker worker)
	{
		_unemployed.Remove(worker);
		worker.GotUnemployed -= OnGotUnemployed;
		worker.GotEmployed -= OnGotEmployed;
		worker.GetComponent<WorkRefuser>().RefusesWorkChanged -= OnRefusesWorkChanged;
	}

	private void OnGotUnemployed(object sender, EventArgs e)
	{
		Worker worker = (Worker)sender;
		UpdateUnemployedState(worker, worker.GetComponent<WorkRefuser>());
	}

	private void OnGotEmployed(object sender, EventArgs e)
	{
		_unemployed.Remove((Worker)sender);
	}

	private void OnRefusesWorkChanged(object sender, EventArgs e)
	{
		WorkRefuser workRefuser = (WorkRefuser)sender;
		UpdateUnemployedState(workRefuser.GetComponent<Worker>(), workRefuser);
	}

	private void UpdateUnemployedState(Worker worker, WorkRefuser workRefuser)
	{
		if (!worker.Employed && !workRefuser.RefusesWork)
		{
			_unemployed.Add(worker);
		}
		else
		{
			_unemployed.Remove(worker);
		}
	}
}
