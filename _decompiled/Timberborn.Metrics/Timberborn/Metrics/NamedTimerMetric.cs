namespace Timberborn.Metrics;

internal class NamedTimerMetric
{
	public string Name { get; }

	public long ElapsedMilliseconds { get; }

	public NamedTimerMetric(string name, long elapsedMilliseconds)
	{
		Name = name;
		ElapsedMilliseconds = elapsedMilliseconds;
	}
}
