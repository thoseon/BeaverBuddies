using Timberborn.AchievementSystem;
using Timberborn.FactionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class UnlockIronTeethAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly FactionSpecService _factionSpecService;

	private readonly FactionUnlockingService _factionUnlockingService;

	public override string Id => "UNLOCK_IRON_TEETH";

	public UnlockIronTeethAchievement(EventBus eventBus, FactionSpecService factionSpecService, FactionUnlockingService factionUnlockingService)
	{
		_eventBus = eventBus;
		_factionSpecService = factionSpecService;
		_factionUnlockingService = factionUnlockingService;
	}

	[OnEvent]
	public void OnFactionUnlocked(FactionUnlockedEvent factionUnlockedEvent)
	{
		if (IsFactionUnlocked())
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		if (IsFactionUnlocked())
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

	private bool IsFactionUnlocked()
	{
		FactionSpec faction = _factionSpecService.GetFaction(AchievementHelper.IronTeeth);
		return !_factionUnlockingService.IsLocked(faction);
	}
}
