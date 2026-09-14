using Timberborn.FactionSystem;
using Timberborn.GameFactionSystem;
using Timberborn.TickSystem;
using Timberborn.Wellbeing;

namespace Timberborn.FactionGoalsSystem;

internal class FactionGoalsUnlocker : ITickableSingleton
{
	private readonly FactionUnlockingService _factionUnlockingService;

	private readonly WellbeingService _wellbeingService;

	private readonly FactionService _factionService;

	private readonly FactionSpecService _factionSpecService;

	public FactionGoalsUnlocker(FactionUnlockingService factionUnlockingService, WellbeingService wellbeingService, FactionService factionService, FactionSpecService factionSpecService)
	{
		_factionUnlockingService = factionUnlockingService;
		_wellbeingService = wellbeingService;
		_factionService = factionService;
		_factionSpecService = factionSpecService;
	}

	public void Tick()
	{
		foreach (FactionSpec faction in _factionSpecService.Factions)
		{
			if (_factionUnlockingService.IsLocked(faction) && UnlockConditionsAreSatisfied(faction))
			{
				_factionUnlockingService.UnlockFaction(faction);
			}
		}
	}

	private bool UnlockConditionsAreSatisfied(FactionSpec factionSpec)
	{
		UnlockableFactionSpec spec = factionSpec.GetSpec<UnlockableFactionSpec>();
		if (_factionService.Current.Id == spec.PrerequisiteFaction)
		{
			return _wellbeingService.AverageGlobalWellbeing >= spec.AverageWellbeingToUnlock;
		}
		return false;
	}
}
