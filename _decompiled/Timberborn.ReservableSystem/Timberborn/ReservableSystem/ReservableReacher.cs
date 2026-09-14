using Timberborn.BaseComponentSystem;
using Timberborn.WalkingSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.ReservableSystem;

public abstract class ReservableReacher : BaseComponent, INamedComponent
{
	public string ComponentName => GetType().Name;

	public abstract IDestination Destination { get; }

	public abstract void NotifyReservableReached(BaseComponent agent);
}
