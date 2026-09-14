using System.Text;
using Timberborn.BonusSystem;
using Timberborn.CoreUI;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.WellbeingUI;
using Timberborn.WorkSystem;

namespace Timberborn.WorkSystemUI;

internal class WorkSystemNeedEffectDescriber : INeedEffectDescriber
{
	private readonly BonusDescriber _bonusDescriber;

	public WorkSystemNeedEffectDescriber(BonusDescriber bonusDescriber)
	{
		_bonusDescriber = bonusDescriber;
	}

	public void DescribeNeedEffects(StringBuilder content, NeedManager needManager, NeedSpec needSpec)
	{
		NeedPreventingWorkSpec workPreventingSpec = GetWorkPreventingSpec(needManager, needSpec);
		if (workPreventingSpec != null)
		{
			string value = workPreventingSpec.WorkRefusalWarning.Value;
			content.AppendLine(" " + SpecialStrings.RowStarter + _bonusDescriber.ColorNegative(value));
		}
	}

	private static NeedPreventingWorkSpec GetWorkPreventingSpec(NeedManager needManager, NeedSpec needSpec)
	{
		NeedPreventingWorkSpec spec = needSpec.GetSpec<NeedPreventingWorkSpec>();
		if ((object)spec != null)
		{
			bool num = !needManager.NeedIsFavorable(needSpec.Id);
			Worker component = needManager.GetComponent<Worker>();
			if (num && (bool)component)
			{
				return spec;
			}
		}
		return null;
	}
}
