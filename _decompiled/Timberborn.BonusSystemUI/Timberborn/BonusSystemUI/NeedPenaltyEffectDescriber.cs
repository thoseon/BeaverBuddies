using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.BonusSystem;
using Timberborn.CoreUI;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.WellbeingUI;

namespace Timberborn.BonusSystemUI;

public class NeedPenaltyEffectDescriber : INeedEffectDescriber
{
	private static readonly string AdultOnlyBonusId = "WorkingSpeed";

	private static readonly string ChildOnlyBonusId = "GrowthSpeed";

	private readonly BonusDescriber _bonusDescriber;

	public NeedPenaltyEffectDescriber(BonusDescriber bonusDescriber)
	{
		_bonusDescriber = bonusDescriber;
	}

	public void DescribeNeedEffects(StringBuilder content, NeedManager needManager, NeedSpec needSpec)
	{
		DescribePenalties(content, needManager, needSpec);
	}

	private void DescribePenalties(StringBuilder content, NeedManager needManager, NeedSpec needSpec)
	{
		PunitiveNeedSpec spec = needSpec.GetSpec<PunitiveNeedSpec>();
		if ((object)spec == null || needManager.NeedIsFavorable(needSpec.Id))
		{
			return;
		}
		foreach (BonusSpec penalty in spec.Penalties)
		{
			if (CanDescribePenalty(needManager, penalty.Id))
			{
				string text = _bonusDescriber.DescribeColored(penalty);
				content.AppendLine(" " + SpecialStrings.RowStarter + text);
			}
		}
	}

	private static bool CanDescribePenalty(NeedManager needManager, string bonusId)
	{
		Child component = ((BaseComponent)(object)needManager).GetComponent<Child>();
		if (bonusId != ChildOnlyBonusId || (bool)(BaseComponent)(object)component)
		{
			if (!(bonusId != AdultOnlyBonusId))
			{
				return !(BaseComponent)(object)component;
			}
			return true;
		}
		return false;
	}
}
