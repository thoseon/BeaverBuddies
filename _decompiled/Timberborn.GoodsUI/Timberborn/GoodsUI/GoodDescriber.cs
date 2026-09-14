using Timberborn.Goods;
using Timberborn.UIFormatters;
using UnityEngine;

namespace Timberborn.GoodsUI;

public class GoodDescriber
{
	private readonly ResourceAmountFormatter _resourceAmountFormatter;

	private readonly IGoodService _goodService;

	public GoodDescriber(ResourceAmountFormatter resourceAmountFormatter, IGoodService goodService)
	{
		_resourceAmountFormatter = resourceAmountFormatter;
		_goodService = goodService;
	}

	public DescribedGood GetDescribedGood(string goodId)
	{
		GoodSpec good = _goodService.GetGood(goodId);
		return new DescribedGood(good.PluralDisplayName.Value, good.IconSmall.Value);
	}

	public string Describe(string goodId)
	{
		return GetDescribedGood(goodId).DisplayName;
	}

	public string Describe(GoodAmount goodAmount)
	{
		return _resourceAmountFormatter.Format(Describe(goodAmount.GoodId), goodAmount.Amount);
	}

	public Sprite GetIcon(string goodId)
	{
		return GetDescribedGood(goodId).Icon;
	}
}
