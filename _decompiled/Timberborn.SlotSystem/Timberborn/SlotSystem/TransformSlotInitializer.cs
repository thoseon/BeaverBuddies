using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class TransformSlotInitializer : SlotInitializer
{
	private readonly SlotRetriever _slotRetriever;

	private readonly TransformSlotFactory _transformSlotFactory;

	private bool _initialized;

	public TransformSlotInitializer(SlotRetriever slotRetriever, TransformSlotFactory transformSlotFactory)
	{
		_slotRetriever = slotRetriever;
		_transformSlotFactory = transformSlotFactory;
	}

	public override IEnumerable<ISlot> InitializeSlots()
	{
		if (_initialized)
		{
			throw new InvalidOperationException("TransformSlotInitializer at " + base.Name + " already initialized its slots");
		}
		_initialized = true;
		return GetComponent<TransformSlotInitializerSpec>().Slots.SelectMany(InitializeSlotsOfSpec);
	}

	private IEnumerable<ISlot> InitializeSlotsOfSpec(TransformSlotSpec spec)
	{
		IEnumerable<Transform> slots = _slotRetriever.GetSlots(base.GameObject, spec.SlotKeyword);
		int i = 0;
		foreach (Transform item in slots)
		{
			int num = i + 1;
			i = num;
			yield return _transformSlotFactory.Create(item, spec);
		}
		if (i == 0)
		{
			throw new InvalidOperationException("There are no \"" + spec.SlotKeyword + "\" slots in " + base.Name);
		}
	}
}
