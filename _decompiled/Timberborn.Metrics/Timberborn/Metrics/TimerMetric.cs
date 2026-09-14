using System.Diagnostics;

namespace Timberborn.Metrics;

internal class TimerMetric : ITimerMetric
{
	private readonly Stopwatch _stopwatch = new Stopwatch();

	public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

	public void Resume()
	{
		_stopwatch.Start();
	}

	public void Pause()
	{
		_stopwatch.Stop();
	}

	public void Reset()
	{
		_stopwatch.Reset();
	}
}
