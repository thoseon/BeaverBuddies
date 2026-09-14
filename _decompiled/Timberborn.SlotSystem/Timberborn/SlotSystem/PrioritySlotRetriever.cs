using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;

namespace Timberborn.SlotSystem;

internal class PrioritySlotRetriever : BaseComponent, IAwakableComponent, ICustomSlotRetriever
{
	private PrioritySlotRetrieverSpec _prioritySlotRetrieverSpec;

	public void Awake()
	{
		_prioritySlotRetrieverSpec = GetComponent<PrioritySlotRetrieverSpec>();
	}

	public bool TryGetUnassignedSlot(IEnumerable<ISlot> slots, out ISlot slot)
	{
		List<ISlot> list = slots.ToList();
		foreach (string prioritySlotName in _prioritySlotRetrieverSpec.PrioritySlotNames)
		{
			foreach (ISlot item in list)
			{
				if (item.Name == prioritySlotName && !item.AssignedEnterer)
				{
					slot = item;
					return true;
				}
			}
		}
		slot = null;
		return false;
	}
}
