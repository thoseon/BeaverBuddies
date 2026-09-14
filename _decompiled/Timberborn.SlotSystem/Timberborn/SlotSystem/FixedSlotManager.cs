using System;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;

namespace Timberborn.SlotSystem;

public class FixedSlotManager : BaseComponent, IAwakableComponent, IPostInitializableEntity, IUpdatableComponent, IFinishedStateListener
{
	private Enterable _enterable;

	private SlotManager _slotManager;

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_slotManager = GetComponent<SlotManager>();
		DisableComponent();
	}

	public void PostInitializeEntity()
	{
		ValidateSlots();
	}

	public void Update()
	{
		_slotManager.UpdateAssignedSlots();
	}

	public void OnEnterFinishedState()
	{
		SubscribeToEvents();
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		UnsubscribeFromEvents();
		DisableComponent();
	}

	private void SubscribeToEvents()
	{
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
	}

	private void UnsubscribeFromEvents()
	{
		_enterable.EntererAdded -= OnEntererAdded;
		_enterable.EntererRemoved -= OnEntererRemoved;
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		if (!_slotManager.AddEnterer(e.Enterer))
		{
			throw new InvalidOperationException($"No unassigned slots left out of total {_slotManager.SlotsCount} at {base.Name}" + $"\n{e.Enterer} tried to enter.\n{_slotManager.GetSlotsOccupation()}");
		}
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		_slotManager.RemoveEnterer(e.Enterer);
	}

	private void ValidateSlots()
	{
		EnterableSpec component = GetComponent<EnterableSpec>();
		if (!component.LimitedCapacityFinished)
		{
			throw new InvalidOperationException("FixedSlotManager does not support unlimited Enterables");
		}
		int capacityFinished = component.CapacityFinished;
		if (_slotManager.SlotsCount < capacityFinished)
		{
			string name = GetComponent<Enterable>().Name;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("There are not enough slots in " + name + " for all visitors!");
			stringBuilder.AppendLine($" There are {_slotManager.SlotsCount} slots but {capacityFinished} visitors.");
			throw new InvalidOperationException(stringBuilder.ToString());
		}
	}
}
