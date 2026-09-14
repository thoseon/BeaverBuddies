using Timberborn.BlueprintSystem;
using Timberborn.PlayerDataSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.FactionSystem;

public class FactionUnlockingService
{
	private static readonly string FactionUnlockKeyPrefix = "FactionUnlocked_";

	private readonly FactionSpecService _factionSpecService;

	private readonly IPlayerDataService _playerDataService;

	private readonly EventBus _eventBus;

	public FactionUnlockingService(FactionSpecService factionSpecService, IPlayerDataService playerDataService, EventBus eventBus)
	{
		_factionSpecService = factionSpecService;
		_playerDataService = playerDataService;
		_eventBus = eventBus;
	}

	public void UnlockFaction(FactionSpec factionSpec)
	{
		Unlock(factionSpec);
		_eventBus.Post(new FactionUnlockedEvent(factionSpec));
	}

	public void UnlockAllFactions()
	{
		foreach (UnlockableFactionSpec unlockableFaction in _factionSpecService.UnlockableFactions)
		{
			Unlock(unlockableFaction.GetSpec<FactionSpec>());
		}
	}

	public void LockAllFactions()
	{
		foreach (UnlockableFactionSpec unlockableFaction in _factionSpecService.UnlockableFactions)
		{
			_playerDataService.Remove(ComposeFactionUnlockKey(unlockableFaction.GetSpec<FactionSpec>().Id));
		}
	}

	public bool IsLocked(ComponentSpec componentSpec)
	{
		if (componentSpec.HasSpec<UnlockableFactionSpec>())
		{
			return !_playerDataService.GetBool(ComposeFactionUnlockKey(componentSpec.GetSpec<FactionSpec>().Id), defaultValue: false);
		}
		return false;
	}

	private void Unlock(FactionSpec factionSpec)
	{
		_playerDataService.SetBool(ComposeFactionUnlockKey(factionSpec.Id), value: true);
	}

	private static string ComposeFactionUnlockKey(string factionId)
	{
		return FactionUnlockKeyPrefix + factionId;
	}
}
