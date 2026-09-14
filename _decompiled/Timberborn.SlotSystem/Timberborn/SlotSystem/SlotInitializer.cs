using System.Collections.Generic;
using Timberborn.BaseComponentSystem;

namespace Timberborn.SlotSystem;

public abstract class SlotInitializer : BaseComponent
{
	public abstract IEnumerable<ISlot> InitializeSlots();
}
