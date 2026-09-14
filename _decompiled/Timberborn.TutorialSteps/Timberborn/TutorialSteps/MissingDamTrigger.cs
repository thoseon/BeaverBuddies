using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class MissingDamTrigger : ILoadableSingleton
{
	private static readonly string TriggerId = "MissingDamTrigger";

	private readonly EventBus _eventBus;

	private readonly ITutorialTriggers _tutorialTriggers;

	private readonly BuiltBuildingService _builtBuildingService;

	public MissingDamTrigger(EventBus eventBus, ITutorialTriggers tutorialTriggers, BuiltBuildingService builtBuildingService)
	{
		_eventBus = eventBus;
		_tutorialTriggers = tutorialTriggers;
		_builtBuildingService = builtBuildingService;
	}

	public void Load()
	{
		if (_tutorialTriggers.TriggerPending(TriggerId))
		{
			_eventBus.Register(this);
		}
	}

	[OnEvent]
	public void OnCycleEnded(CycleEndedEvent cycleEndedEvent)
	{
		if (cycleEndedEvent.Cycle == 3 && _builtBuildingService.NumberOfAllBuildings(new string[6] { "Dam.Folktails", "Floodgate.Folktails", "DoubleFloodgate.Folktails", "TripleFloodgate.Folktails", "ThrottlingValve.Folktails", "FillValve.Folktails" }) == 0)
		{
			_eventBus.Unregister(this);
			_tutorialTriggers.AddTrigger(TriggerId);
		}
	}
}
