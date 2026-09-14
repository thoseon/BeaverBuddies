using System.Text;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.GoodsUI;

namespace Timberborn.Effects;

public class GoodEffectDescriber
{
	private readonly EffectDescriber _effectDescriber;

	private readonly IGoodService _goodService;

	private readonly GoodDescriber _goodDescriber;

	private readonly StringBuilder _description = new StringBuilder();

	public GoodEffectDescriber(EffectDescriber effectDescriber, IGoodService goodService, GoodDescriber goodDescriber)
	{
		_effectDescriber = effectDescriber;
		_goodService = goodService;
		_goodDescriber = goodDescriber;
	}

	public string DescribeEffectsWithHeader(string goodId)
	{
		return DescribeEffectsWithHeader(goodId, _goodDescriber.Describe(goodId));
	}

	public string DescribeEffects(string goodId)
	{
		_description.Clear();
		DescribeEffects(goodId, _description);
		return _description.ToStringWithoutNewLineEnd();
	}

	public void DescribeEffects(string goodId, StringBuilder description)
	{
		DescribeEffects(_goodService.GetGood(goodId), description);
	}

	private string DescribeEffectsWithHeader(string goodId, string header)
	{
		_description.Clear();
		_description.AppendLine(header);
		DescribeEffects(goodId, _description);
		return _description.ToStringWithoutNewLineEnd();
	}

	private void DescribeEffects(GoodSpec goodSpec, StringBuilder description)
	{
		if (goodSpec.ConsumptionEffects.Length > 0)
		{
			_effectDescriber.DescribeEffects(goodSpec.ConsumptionEffects, description);
		}
	}
}
