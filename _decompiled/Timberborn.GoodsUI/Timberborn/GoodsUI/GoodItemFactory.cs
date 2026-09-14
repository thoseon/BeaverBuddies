using Timberborn.Goods;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.GoodsUI;

public class GoodItemFactory
{
	private readonly DescribedAmountFactory _describedAmountFactory;

	private readonly GoodDescriber _goodDescriber;

	public GoodItemFactory(DescribedAmountFactory describedAmountFactory, GoodDescriber goodDescriber)
	{
		_describedAmountFactory = describedAmountFactory;
		_goodDescriber = goodDescriber;
	}

	public VisualElement Create(GoodAmountSpec goodAmount, bool bordered)
	{
		DescribedGood describedGood = _goodDescriber.GetDescribedGood(goodAmount.Id);
		if (bordered)
		{
			return _describedAmountFactory.CreateBordered(goodAmount.Amount.ToString(), describedGood.Icon, describedGood.DisplayName);
		}
		return _describedAmountFactory.CreatePlain("", goodAmount.Amount.ToString(), describedGood.Icon, describedGood.DisplayName);
	}
}
