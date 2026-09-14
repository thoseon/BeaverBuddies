using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class PatrollingSlotInitializer : SlotInitializer
{
	private readonly SlotRetriever _slotRetriever;

	private readonly PatrollingSlotFactory _patrollingSlotFactory;

	private bool _initialized;

	public PatrollingSlotInitializer(SlotRetriever slotRetriever, PatrollingSlotFactory patrollingSlotFactory)
	{
		_slotRetriever = slotRetriever;
		_patrollingSlotFactory = patrollingSlotFactory;
	}

	public override IEnumerable<ISlot> InitializeSlots()
	{
		if (_initialized)
		{
			throw new InvalidOperationException("PatrollingSlotInitializer at " + base.Name + " already initialized its slots");
		}
		_initialized = true;
		return GetComponent<PatrollingSlotInitializerSpec>().PatrollingSlots.SelectMany(InitializeSlotsOfSpec);
	}

	private IEnumerable<ISlot> InitializeSlotsOfSpec(PatrollingSlotSpec spec)
	{
		IEnumerable<Transform> slots = _slotRetriever.GetSlots(base.GameObject, spec.SlotKeyword);
		int i = 0;
		foreach (Transform item in slots)
		{
			yield return CreateSlot(item.gameObject, spec, string.Format("{0}{1}", "PatrollingSlot", i++));
		}
		if (i == 0)
		{
			throw new InvalidOperationException("There are no \"" + spec.SlotKeyword + "\" slots in " + base.Name);
		}
	}

	private PatrollingSlot CreateSlot(GameObject emptyInModel, PatrollingSlotSpec spec, string slotObjectName)
	{
		(Transform start, Transform end) startAndEnd = _slotRetriever.GetStartAndEnd(emptyInModel);
		Transform item = startAndEnd.start;
		Transform item2 = startAndEnd.end;
		Transform transform = new GameObject(slotObjectName).transform;
		transform.parent = base.Transform;
		return _patrollingSlotFactory.Create(transform, item, item2, spec);
	}
}
