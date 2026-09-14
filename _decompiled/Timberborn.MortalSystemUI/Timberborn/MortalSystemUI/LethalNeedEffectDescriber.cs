using System.Text;
using Timberborn.BonusSystem;
using Timberborn.CoreUI;
using Timberborn.MortalSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.WellbeingUI;

namespace Timberborn.MortalSystemUI;

internal class LethalNeedEffectDescriber : INeedEffectDescriber
{
	private readonly BonusDescriber _bonusDescriber;

	public LethalNeedEffectDescriber(BonusDescriber bonusDescriber)
	{
		_bonusDescriber = bonusDescriber;
	}

	public void DescribeNeedEffects(StringBuilder content, NeedManager needManager, NeedSpec needSpec)
	{
		if (!needManager.NeedIsFavorable(needSpec.Id))
		{
			LethalNeedSpec spec = needSpec.GetSpec<LethalNeedSpec>();
			if ((object)spec != null)
			{
				string value = spec.DeathWarning.Value;
				content.AppendLine(" " + SpecialStrings.RowStarter + _bonusDescriber.ColorNegative(value));
			}
		}
	}
}
