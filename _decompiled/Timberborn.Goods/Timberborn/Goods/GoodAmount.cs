namespace Timberborn.Goods;

public readonly struct GoodAmount
{
	public string GoodId { get; }

	public int Amount { get; }

	public GoodAmount(string goodId, int amount)
	{
		GoodId = goodId;
		Amount = amount;
	}

	public override string ToString()
	{
		return $"{Amount}x {GoodId}";
	}
}
