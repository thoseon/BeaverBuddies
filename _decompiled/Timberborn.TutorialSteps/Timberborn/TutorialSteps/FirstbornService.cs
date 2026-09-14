using Timberborn.Beavers;
using Timberborn.SingletonSystem;

namespace Timberborn.TutorialSteps;

internal class FirstbornService : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	public bool FirstbornBorn { get; private set; }

	public FirstbornService(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBeaverBorn(BeaverBornEvent beaverBornEvent)
	{
		FirstbornBorn = true;
		_eventBus.Unregister(this);
	}
}
