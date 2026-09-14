using System.Linq;
using Timberborn.LevelVisibilitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class VisibleLevelChangeService : ILoadableSingleton
{
	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly EventBus _eventBus;

	public bool WasAtZero { get; private set; }

	public int LevelsIncreasedSinceZero { get; private set; }

	public bool IsAtMax => _levelVisibilityService.LevelIsAtMax;

	public VisibleLevelChangeService(ILevelVisibilityService levelVisibilityService, EventBus eventBus)
	{
		_levelVisibilityService = levelVisibilityService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnTutorialStageStarted(TutorialStageStartedEvent tutorialStageStartedEvent)
	{
		if (tutorialStageStartedEvent.TutorialStage.TutorialSteps.Any((TutorialStep step) => step.Step is VisibleLevelChangeStep))
		{
			LevelsIncreasedSinceZero = 0;
			WasAtZero = false;
		}
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		int num = _levelVisibilityService.MaxVisibleLevel - maxVisibleLevelChangedEvent.OldMaxVisibleLevel;
		WasAtZero = WasAtZero || _levelVisibilityService.MaxVisibleLevel == 0;
		if (WasAtZero && num > 0)
		{
			LevelsIncreasedSinceZero += num;
		}
	}
}
