using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.SlotSystem;

internal class UnfinishedStateSlotDisabler : BaseComponent, IAwakableComponent, IUnfinishedStateListener
{
	private readonly SlotRetriever _slotRetriever;

	private ImmutableArray<Transform> _slots;

	public UnfinishedStateSlotDisabler(SlotRetriever slotRetriever)
	{
		_slotRetriever = slotRetriever;
	}

	public void Awake()
	{
		UnfinishedStateSlotDisablerSpec component = GetComponent<UnfinishedStateSlotDisablerSpec>();
		_slots = _slotRetriever.GetSlots(base.GameObject, component.SlotKeyword).ToImmutableArray();
	}

	public void OnEnterUnfinishedState()
	{
		foreach (Transform slot in _slots)
		{
			slot.gameObject.SetActive(value: false);
		}
	}

	public void OnExitUnfinishedState()
	{
		foreach (Transform slot in _slots)
		{
			slot.gameObject.SetActive(value: true);
		}
	}
}
