using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;
using Timberborn.Wonders;

namespace Timberborn.Achievements;

internal class BuildWonderBeforeCycleAchievement : Achievement
{
	private static readonly int ThresholdCycle = 15;

	private readonly EventBus _eventBus;

	private readonly GameCycleService _gameCycleService;

	public override string Id => "BUILD_WONDER_BEFORE_CYCLE";

	protected BuildWonderBeforeCycleAchievement(EventBus eventBus, GameCycleService gameCycleService)
	{
		_eventBus = eventBus;
		_gameCycleService = gameCycleService;
	}

	[OnEvent]
	public void OnCycleStarted(CycleStartedEvent cycleStartedEvent)
	{
		if (_gameCycleService.Cycle > ThresholdCycle)
		{
			Disable();
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (_gameCycleService.Cycle <= ThresholdCycle && (bool)enteredFinishedStateEvent.BlockObject.GetComponent<Wonder>())
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}
}
