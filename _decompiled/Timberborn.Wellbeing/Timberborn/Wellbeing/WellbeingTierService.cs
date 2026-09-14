using System.Collections.Generic;
using System.Linq;
using Timberborn.Beavers;
using Timberborn.BlueprintSystem;
using Timberborn.Bots;
using Timberborn.SingletonSystem;

namespace Timberborn.Wellbeing;

internal class WellbeingTierService : IWellbeingTierService, ILoadableSingleton
{
	private readonly ISpecService _specService;

	private Dictionary<string, WellbeingTier> _adultWellbeingTiers;

	private Dictionary<string, WellbeingTier> _childWellbeingTiers;

	private Dictionary<string, WellbeingTier> _botWellbeingTiers;

	public WellbeingTierService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_adultWellbeingTiers = CreateWellbeingTiers("BeaverAdult");
		_childWellbeingTiers = CreateWellbeingTiers("BeaverChild");
		_botWellbeingTiers = CreateWellbeingTiers("Bot");
	}

	public IEnumerable<string> GetTierableBonuses(WellbeingTracker wellbeingTracker)
	{
		return GetWellbeingTiers(wellbeingTracker).Keys;
	}

	public bool TryGetTierBonus(WellbeingTracker wellbeingTracker, string bonusId, int wellbeing, out WellbeingTierBonus tierBonus)
	{
		if (GetWellbeingTiers(wellbeingTracker).TryGetValue(bonusId, out var value))
		{
			return value.TryGetTierBonus(wellbeing, out tierBonus);
		}
		tierBonus = default(WellbeingTierBonus);
		return false;
	}

	public bool TryGetNextTierBonus(WellbeingTracker wellbeingTracker, string bonusId, int wellbeing, out WellbeingTierBonus nextTierBonus)
	{
		if (GetWellbeingTiers(wellbeingTracker).TryGetValue(bonusId, out var value))
		{
			return value.TryGetNextTierBonus(wellbeing, out nextTierBonus);
		}
		nextTierBonus = default(WellbeingTierBonus);
		return false;
	}

	private Dictionary<string, WellbeingTier> CreateWellbeingTiers(string characterType)
	{
		return (from spec in _specService.GetSpecs<WellbeingTierSpec>()
			where spec.CharacterType == characterType
			select spec).ToDictionary((WellbeingTierSpec spec) => spec.BonusId, WellbeingTier.Create);
	}

	private Dictionary<string, WellbeingTier> GetWellbeingTiers(WellbeingTracker wellbeingTracker)
	{
		if (wellbeingTracker.HasComponent<BotSpec>())
		{
			return _botWellbeingTiers;
		}
		if (!wellbeingTracker.GetComponent<Child>())
		{
			return _adultWellbeingTiers;
		}
		return _childWellbeingTiers;
	}
}
