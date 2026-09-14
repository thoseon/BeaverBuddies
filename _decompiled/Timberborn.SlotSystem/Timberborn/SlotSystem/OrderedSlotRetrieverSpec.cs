using System.Collections.Generic;
using JetBrains.Annotations;
using Timberborn.BlueprintSystem;

namespace Timberborn.SlotSystem;

[UsedImplicitly]
internal record OrderedSlotRetrieverSpec : ComponentSpec, ICustomSlotRetriever
{
	public bool TryGetUnassignedSlot(IEnumerable<ISlot> slots, out ISlot slot)
	{
		foreach (ISlot slot2 in slots)
		{
			if (!slot2.AssignedEnterer)
			{
				slot = slot2;
				return true;
			}
		}
		slot = null;
		return false;
	}
}
