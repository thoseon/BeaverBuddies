namespace Timberborn.Goods;

public readonly struct StorableGoodAmount
{
	public StorableGood StorableGood { get; }

	public int Amount { get; }

	public StorableGoodAmount(StorableGood storableGood, int amount)
	{
		StorableGood = storableGood;
		Amount = amount;
	}

	public override string ToString()
	{
		return $"{Amount}x {StorableGood.GoodId}";
	}
}
