using Timberborn.GameCycleSystem;
using Timberborn.Population;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class UnemployedBeaversTrigger : ILoadableSingleton
{
	private static readonly int CycleThreshold = 3;

	private static readonly int UnemployedBeaverThreshold = 4;

	private static readonly int AdultBeaverThreshold = 40;

	private static readonly string TriggerId = "UnemployedBeaversTrigger";

	private readonly EventBus _eventBus;

	private readonly ITutorialTriggers _tutorialTriggers;

	private readonly PopulationService _populationService;

	private readonly GameCycleService _gameCycleService;

	public UnemployedBeaversTrigger(EventBus eventBus, ITutorialTriggers tutorialTriggers, PopulationService populationService, GameCycleService gameCycleService)
	{
		_eventBus = eventBus;
		_tutorialTriggers = tutorialTriggers;
		_populationService = populationService;
		_gameCycleService = gameCycleService;
	}

	public void Load()
	{
		if (_tutorialTriggers.TriggerPending(TriggerId))
		{
			_eventBus.Register(this);
		}
	}

	[OnEvent]
	public void OnPopulationChangedEvent(PopulationChangedEvent populationChangedEvent)
	{
		PopulationData globalPopulationData = _populationService.GlobalPopulationData;
		if (_gameCycleService.Cycle >= CycleThreshold && (globalPopulationData.NumberOfAdults >= AdultBeaverThreshold || globalPopulationData.BeaverWorkplaceData.Unemployed >= UnemployedBeaverThreshold))
		{
			_eventBus.Unregister(this);
			_tutorialTriggers.AddTrigger(TriggerId);
		}
	}
}
