using Timberborn.HazardousWeatherSystem;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class SurvivedFirstBadtideTrigger : ILoadableSingleton
{
	private static readonly string TriggerId = "SurvivedFirstBadtideTrigger";

	private readonly EventBus _eventBus;

	private readonly ITutorialTriggers _tutorialTriggers;

	public SurvivedFirstBadtideTrigger(EventBus eventBus, ITutorialTriggers tutorialTriggers)
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
		if (hazardousWeatherEndedEvent.HazardousWeather is BadtideWeather)
		{
			_eventBus.Unregister(this);
			_tutorialTriggers.AddTrigger(TriggerId);
		}
	}
}
