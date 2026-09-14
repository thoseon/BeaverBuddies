using Timberborn.BaseComponentSystem;

namespace Timberborn.TickSystem;

public abstract class TickableComponent : BaseComponent
{
	public abstract void Tick();
}
