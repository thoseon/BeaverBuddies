namespace Timberborn.GoodsSampling;

public readonly struct GoodSample
{
	public int Cycle { get; }

	public int Day { get; }

	public int Stock { get; }

	public int Capacity { get; }

	public int Production { get; }

	public int Consumption { get; }

	public GoodSample(int cycle, int day, int stock, int capacity, int production, int consumption)
	{
		Cycle = cycle;
		Day = day;
		Stock = stock;
		Capacity = capacity;
		Production = production;
		Consumption = consumption;
	}

	public static GoodSample operator +(GoodSample left, GoodSample right)
	{
		return new GoodSample((left.Cycle == 0) ? right.Cycle : left.Cycle, (left.Day == 0) ? right.Day : left.Day, left.Stock + right.Stock, left.Capacity + right.Capacity, left.Production + right.Production, left.Consumption + right.Consumption);
	}
}
