using Timberborn.AchievementSystem;
using Timberborn.GameCycleSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal abstract class CycleSurvivalAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly GameCycleService _gameCycleService;

	private readonly int _thresholdCycle;

	public override string Id => $"SURVIVE_{_thresholdCycle}_CYCLES";

	private bool CycleIsAboveThreshold => _gameCycleService.Cycle > _thresholdCycle;

	protected CycleSurvivalAchievement(EventBus eventBus, GameCycleService gameCycleService, int thresholdCycle)
	{
		_eventBus = eventBus;
		_gameCycleService = gameCycleService;
		_thresholdCycle = thresholdCycle;
	}

	[OnEvent]
	public void OnCycleStarted(CycleStartedEvent cycleStartedEvent)
	{
		if (CycleIsAboveThreshold)
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		if (CycleIsAboveThreshold)
		{
			Unlock();
		}
		else
		{
			_eventBus.Register(this);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}
}
