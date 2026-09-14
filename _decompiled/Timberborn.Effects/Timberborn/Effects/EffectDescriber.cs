using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.CoreUI;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.Effects;

public class EffectDescriber
{
	private readonly FactionNeedService _factionNeedService;

	private readonly NeedSpecFormatter _needSpecFormatter;

	public EffectDescriber(FactionNeedService factionNeedService, NeedSpecFormatter needSpecFormatter)
	{
		_factionNeedService = factionNeedService;
		_needSpecFormatter = needSpecFormatter;
	}

	public void DescribeEffects(IEnumerable<InstantEffectSpec> effects, StringBuilder description)
	{
		DescribeEffects(effects.Select((InstantEffectSpec effect) => effect.NeedId), description);
	}

	public void DescribeEffects(IEnumerable<ContinuousEffectSpec> effects, StringBuilder description)
	{
		DescribeEffects(effects.Select((ContinuousEffectSpec effect) => effect.NeedId), description);
	}

	public void DescribeRangeEffects(IEnumerable<ContinuousEffectSpec> effects, StringBuilder stringBuilder, int range)
	{
		DescribeEffects(effects.Select((ContinuousEffectSpec effect) => effect.NeedId), stringBuilder, range);
	}

	private void DescribeEffects(IEnumerable<string> needIds, StringBuilder description, int range = 0)
	{
		foreach (string needId in needIds)
		{
			description.Append(SpecialStrings.RowStarter);
			NeedSpec beaverOrBotNeedById = _factionNeedService.GetBeaverOrBotNeedById(needId);
			description.Append((range > 0) ? _needSpecFormatter.FormatRangedNeed(beaverOrBotNeedById, range) : _needSpecFormatter.FormatNeed(beaverOrBotNeedById));
			description.AppendLine();
		}
	}
}
