using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class SurvivedFirstDroughtTrigger : ILoadableSingleton
{
	private static readonly string TriggerId = "SurvivedFirstDroughtTrigger";

	private readonly EventBus _eventBus;

	private readonly ITutorialTriggers _tutorialTriggers;

	public SurvivedFirstDroughtTrigger(EventBus eventBus, ITutorialTriggers tutorialTriggers)
	{
		_eventBus = eventBus;
		_tutorialTriggers = tutorialTriggers;
	}

	public void Load()
	{
		if (_tutorialTriggers.TriggerPending(TriggerId))
		{
			_eventBus.Register(this);
		}
	}

	[OnEvent]
	public void OnHazardousWeatherEnded(HazardousWeatherEndedEvent hazardousWeatherEndedEvent)
	{
		if (hazardousWeatherEndedEvent.HazardousWeather is DroughtWeather)
		{
			_eventBus.Unregister(this);
			_tutorialTriggers.AddTrigger(TriggerId);
		}
	}
}
