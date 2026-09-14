using System;
using System.Collections.Generic;
using System.Linq;

namespace Timberborn.Wellbeing;

internal class WellbeingTier
{
	private static readonly float BonusComparisionTolerance = 1E-05f;

	private readonly WellbeingTierSpec _tierSpec;

	private readonly List<WellbeingTierBonus> _predefinedBonuses = new List<WellbeingTierBonus>();

	private WellbeingTierBonus _lastPredefinedTier;

	private WellbeingTier(WellbeingTierSpec tierSpec)
	{
		_tierSpec = tierSpec;
	}

	public static WellbeingTier Create(WellbeingTierSpec tierSpec)
	{
		WellbeingTier wellbeingTier = new WellbeingTier(tierSpec);
		wellbeingTier.CachePredefinedBonuses();
		return wellbeingTier;
	}

	public bool TryGetTierBonus(int wellbeing, out WellbeingTierBonus tierBonus)
	{
		if (wellbeing >= 0)
		{
			tierBonus = ((wellbeing < _predefinedBonuses.Count) ? GetPredefinedBonus(wellbeing) : GetCalculatedBonus(wellbeing));
			return true;
		}
		tierBonus = default(WellbeingTierBonus);
		return false;
	}

	public bool TryGetNextTierBonus(int wellbeing, out WellbeingTierBonus nextTierBonus)
	{
		if (wellbeing >= 0)
		{
			nextTierBonus = ((wellbeing < _predefinedBonuses.Count - 1) ? GetPredefinedNextBonus(wellbeing) : GetCalculatedNextBonus(wellbeing));
			return true;
		}
		nextTierBonus = default(WellbeingTierBonus);
		return false;
	}

	private void CachePredefinedBonuses()
	{
		WellbeingTierBonus item = default(WellbeingTierBonus);
		int num = _tierSpec.Bonuses.Max((WellbeingTierBonusSpec bonus) => bonus.Wellbeing);
		int i;
		for (i = 0; i <= num; i++)
		{
			WellbeingTierBonusSpec wellbeingTierBonusSpec = _tierSpec.Bonuses.FirstOrDefault((WellbeingTierBonusSpec bonus) => bonus.Wellbeing == i);
			if (wellbeingTierBonusSpec != null)
			{
				item = new WellbeingTierBonus(wellbeingTierBonusSpec.Wellbeing, wellbeingTierBonusSpec.Multiplier);
			}
			_predefinedBonuses.Add(item);
		}
		_lastPredefinedTier = _predefinedBonuses.Last();
	}

	private WellbeingTierBonus GetPredefinedBonus(int wellbeing)
	{
		return _predefinedBonuses[wellbeing];
	}

	private WellbeingTierBonus GetCalculatedBonus(int wellbeing)
	{
		int wellbeing2 = _lastPredefinedTier.Wellbeing;
		float bonus = _lastPredefinedTier.Bonus;
		int num = (wellbeing - wellbeing2) / _tierSpec.WellbeingThreshold;
		float bonus2 = bonus + (float)num * _tierSpec.MultiplierIncrement;
		return new WellbeingTierBonus(wellbeing2 + num * _tierSpec.WellbeingThreshold, bonus2);
	}

	private WellbeingTierBonus GetPredefinedNextBonus(int wellbeing)
	{
		float bonus = _predefinedBonuses[wellbeing].Bonus;
		for (int i = wellbeing + 1; i < _predefinedBonuses.Count; i++)
		{
			float bonus2 = _predefinedBonuses[i].Bonus;
			if (Math.Abs(bonus2 - bonus) > BonusComparisionTolerance)
			{
				return new WellbeingTierBonus(i, bonus2);
			}
		}
		return default(WellbeingTierBonus);
	}

	private WellbeingTierBonus GetCalculatedNextBonus(int wellbeing)
	{
		int wellbeing2 = _lastPredefinedTier.Wellbeing;
		float bonus = _lastPredefinedTier.Bonus;
		int num = (wellbeing - wellbeing2) / _tierSpec.WellbeingThreshold;
		int wellbeing3 = wellbeing2 + (num + 1) * _tierSpec.WellbeingThreshold;
		float bonus2 = bonus + (float)(num + 1) * _tierSpec.MultiplierIncrement;
		return new WellbeingTierBonus(wellbeing3, bonus2);
	}
}
