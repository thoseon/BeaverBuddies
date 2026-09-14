using Timberborn.BaseComponentSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.BehaviorSystem;

public abstract class Behavior : BaseComponent, INamedComponent
{
	public string ComponentName => GetType().Name;

	public abstract Decision Decide(BehaviorAgent agent);
}
