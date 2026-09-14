using Timberborn.AchievementSystem;
using Timberborn.GameFactionSystem;

namespace Timberborn.Achievements;

internal class InjuredJustBornBeaverAchievement : Achievement
{
	private readonly FactionService _factionService;

	public bool CanTrackInjury { get; private set; }

	public override string Id => "INJURED_JUST_BORN_BEAVER";

	public InjuredJustBornBeaverAchievement(FactionService factionService)
	{
		_factionService = factionService;
	}

	protected override void EnableInternal()
	{
		if (_factionService.Current.Id == AchievementHelper.IronTeeth)
		{
			CanTrackInjury = true;
		}
	}

	protected override void DisableInternal()
	{
		CanTrackInjury = false;
	}
}
