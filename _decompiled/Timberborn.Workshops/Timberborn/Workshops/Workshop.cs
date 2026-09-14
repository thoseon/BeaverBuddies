using System;
using Timberborn.BaseComponentSystem;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Timberborn.Workshops;

public class Workshop : BaseComponent, IAwakableComponent
{
	private Workplace _workplace;

	public int NumberOfWorkersWorking { get; private set; }

	public bool CurrentlyWorking => NumberOfWorkersWorking > 0;

	public event EventHandler<WorkshopStateChangedEventArgs> WorkshopStateChanged;

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
	}

	internal void InformOfStartedWorking()
	{
		if (NumberOfWorkersWorking >= _workplace.MaxWorkers)
		{
			Debug.LogWarning("Can't have a number of working workers higher than the max number of workers at " + base.Name);
			return;
		}
		int numberOfWorkersWorking = NumberOfWorkersWorking + 1;
		NumberOfWorkersWorking = numberOfWorkersWorking;
		InvokeWorkshopStateChangedEvent();
	}

	internal void InformOfStoppedWorking()
	{
		if (NumberOfWorkersWorking <= 0)
		{
			Debug.LogWarning("Can't have a negative number of working workers at " + base.Name);
			return;
		}
		int numberOfWorkersWorking = NumberOfWorkersWorking - 1;
		NumberOfWorkersWorking = numberOfWorkersWorking;
		InvokeWorkshopStateChangedEvent();
	}

	private void InvokeWorkshopStateChangedEvent()
	{
		WorkshopStateChangedEventArgs e = new WorkshopStateChangedEventArgs(CurrentlyWorking);
		WorkshopStateChanged?.Invoke(this, e);
	}
}
