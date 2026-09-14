using System.Collections.Generic;

namespace Timberborn.SlotSystem;

public interface ICustomSlotRetriever
{
	bool TryGetUnassignedSlot(IEnumerable<ISlot> slots, out ISlot slot);
}
