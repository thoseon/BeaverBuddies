using Timberborn.BlueprintSystem;

namespace Timberborn.Goods;

public record GoodAmountSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public int Amount { get; init; }

	public GoodAmount ToGoodAmount()
	{
		return new GoodAmount(Id, Amount);
	}

	public override string ToString()
	{
		return $"{Amount}x {Id}";
	}
}
