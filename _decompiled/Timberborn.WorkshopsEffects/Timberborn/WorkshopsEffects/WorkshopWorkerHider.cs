using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EnterableSystem;
using Timberborn.SlotSystem;
using Timberborn.TickSystem;

namespace Timberborn.WorkshopsEffects;

internal class WorkshopWorkerHider : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly List<WorkshopWorker> _workerInSlots = new List<WorkshopWorker>();

	public void Awake()
	{
		SlotManager component = GetComponent<SlotManager>();
		component.EntererAssignedToSlot += OnEntererAssignedToSlot;
		component.EntererUnassignedFromSlot += delegate(object _, Enterer e)
		{
			_workerInSlots.Remove(e.GetComponent<WorkshopWorker>());
		};
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public override void Tick()
	{
		UpdateVisibility();
	}

	private void OnEntererAssignedToSlot(object sender, Enterer e)
	{
		WorkshopWorker component = e.GetComponent<WorkshopWorker>();
		_workerInSlots.Add(component);
		component.UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		foreach (WorkshopWorker workerInSlot in _workerInSlots)
		{
			workerInSlot.UpdateVisibility();
		}
	}
}
