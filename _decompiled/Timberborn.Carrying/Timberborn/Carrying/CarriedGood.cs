using Timberborn.Goods;

namespace Timberborn.Carrying;

public readonly struct CarriedGood
{
	public static readonly CarriedGood Empty = new CarriedGood(new GoodAmount(null, 0), CarriedGoodType.Uncountable);

	public GoodAmount GoodAmount { get; }

	public CarriedGoodType Type { get; }

	public bool IsEmpty => GoodAmount.Amount == 0;

	public CarriedGood(GoodAmount goodAmount, CarriedGoodType type)
	{
		GoodAmount = goodAmount;
		Type = type;
	}
}
