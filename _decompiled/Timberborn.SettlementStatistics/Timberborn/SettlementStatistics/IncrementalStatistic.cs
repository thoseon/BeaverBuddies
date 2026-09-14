namespace Timberborn.SettlementStatistics;

public class IncrementalStatistic
{
	public string Id { get; }

	public int Value { get; private set; }

	public IncrementalStatistic(string id, int value)
	{
		Id = id;
		Value = value;
	}

	public void Increment()
	{
		Value++;
	}
}
