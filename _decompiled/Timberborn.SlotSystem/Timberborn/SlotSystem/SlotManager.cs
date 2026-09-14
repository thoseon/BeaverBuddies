using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class SlotManager : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private ICustomSlotRetriever _customSlotRetriever;

	private readonly List<ISlot> _slots = new List<ISlot>();

	private readonly HashSet<Enterer> _unassignedEnterers = new HashSet<Enterer>();

	public int SlotsCount => _slots.Count;

	private IEnumerable<ISlot> AvailableSlots => _slots.Where((ISlot slot) => slot.IsAvailable);

	public event EventHandler<Enterer> EntererUnassignedFromSlot;

	public event EventHandler<Enterer> EntererAssignedToSlot;

	public SlotManager(IRandomNumberGenerator randomNumberGenerator)
	{
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_customSlotRetriever = GetComponent<ICustomSlotRetriever>();
	}

	public void InitializeEntity()
	{
		InitializeSlots();
	}

	public void DeleteEntity()
	{
		ClearSlots();
	}

	public void UpdateAssignedSlots()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < _slots.Count; i++)
		{
			ISlot slot = _slots[i];
			if ((bool)slot.AssignedEnterer)
			{
				slot.Update(deltaTime);
			}
		}
	}

	public void ReassignAllSlots()
	{
		foreach (ISlot slot in _slots)
		{
			Enterer assignedEnterer = slot.AssignedEnterer;
			if ((bool)assignedEnterer)
			{
				slot.UnassignEnterer();
				EntererUnassignedFromSlot?.Invoke(this, assignedEnterer);
				AddEnterer(assignedEnterer);
			}
		}
		for (int i = 0; i < _unassignedEnterers.Count; i++)
		{
			AssignFirstUnassigned();
		}
	}

	public bool AddEnterer(Enterer enterer)
	{
		if (TryGetUnassignedSlot(out var unassignedSlot))
		{
			unassignedSlot.AssignEnterer(enterer);
			EntererAssignedToSlot?.Invoke(this, enterer);
			return true;
		}
		_unassignedEnterers.Add(enterer);
		return false;
	}

	public void RemoveEnterer(Enterer enterer)
	{
		ISlot slot = _slots.Find((ISlot slot2) => slot2.AssignedEnterer == enterer);
		if (slot != null)
		{
			Unassign(slot);
			AssignFirstUnassigned();
		}
		else
		{
			_unassignedEnterers.Remove(enterer);
		}
	}

	public string GetSlotsOccupation()
	{
		return _slots.CollectionToString("Slots occupied by: ");
	}

	private void ClearSlots()
	{
		foreach (ISlot slot in _slots)
		{
			Unassign(slot);
		}
		_slots.Clear();
		_unassignedEnterers.Clear();
	}

	private void InitializeSlots()
	{
		foreach (SlotInitializer item in GetComponentsAllocating<SlotInitializer>())
		{
			_slots.AddRange(item.InitializeSlots());
		}
	}

	private bool TryGetUnassignedSlot(out ISlot unassignedSlot)
	{
		if (_customSlotRetriever != null && _customSlotRetriever.TryGetUnassignedSlot(AvailableSlots, out unassignedSlot))
		{
			return true;
		}
		IEnumerable<ISlot> source = AvailableSlots.Where((ISlot slot) => slot.IsAvailable && !slot.AssignedEnterer);
		return _randomNumberGenerator.TryGetEnumerableElement(source, out unassignedSlot);
	}

	private void Unassign(ISlot slot)
	{
		Enterer assignedEnterer = slot.AssignedEnterer;
		if ((bool)assignedEnterer)
		{
			slot.UnassignEnterer();
			EntererUnassignedFromSlot?.Invoke(this, assignedEnterer);
		}
	}

	private void AssignFirstUnassigned()
	{
		if (_unassignedEnterers.Count > 0)
		{
			Enterer enterer = _unassignedEnterers.First();
			_unassignedEnterers.Remove(enterer);
			AddEnterer(enterer);
		}
	}
}
