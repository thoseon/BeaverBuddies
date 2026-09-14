using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Timberborn.Common;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;

namespace Timberborn.WellbeingUI;

internal class NeedEffectDescriptionService
{
	private readonly ImmutableArray<INeedEffectDescriber> _needEffectDescribers;

	private readonly StringBuilder _contentBuilder = new StringBuilder();

	public NeedEffectDescriptionService(IEnumerable<INeedEffectDescriber> needEffectDescribers)
	{
		_needEffectDescribers = needEffectDescribers.ToImmutableArray();
	}

	public string GetNeedDescription(NeedSpec needSpec, NeedManager needManager)
	{
		_contentBuilder.Clear();
		DescribeEffects(_contentBuilder, needSpec, needManager);
		return _contentBuilder.ToStringWithoutNewLineEnd() ?? "";
	}

	private void DescribeEffects(StringBuilder content, NeedSpec needSpec, NeedManager needManager)
	{
		foreach (INeedEffectDescriber needEffectDescriber in _needEffectDescribers)
		{
			needEffectDescriber.DescribeNeedEffects(content, needManager, needSpec);
		}
	}
}
