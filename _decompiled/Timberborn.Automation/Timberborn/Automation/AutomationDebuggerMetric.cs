using System.Diagnostics;

namespace Timberborn.Automation;

public class AutomationDebuggerMetric
{
	public double Total { get; private set; }

	public double Max { get; private set; }

	public void Register(Stopwatch stopwatch)
	{
		double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
		Total += totalMilliseconds;
		if (totalMilliseconds > Max)
		{
			Max = totalMilliseconds;
		}
	}

	public void Reset()
	{
		Total = 0.0;
		Max = 0.0;
	}
}
