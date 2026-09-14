using System.Collections.Generic;
using System.Linq;

namespace Timberborn.Metrics;

internal class MetricsContext
{
	private readonly Dictionary<string, TimerMetric> _timerMetrics = new Dictionary<string, TimerMetric>();

	public ITimerMetric GetTimer(string key)
	{
		if (!_timerMetrics.TryGetValue(key, out var value))
		{
			value = new TimerMetric();
			_timerMetrics[key] = value;
		}
		return value;
	}

	public IEnumerable<NamedTimerMetric> GetAllTimers()
	{
		return _timerMetrics.Select((KeyValuePair<string, TimerMetric> keyAndTimer) => new NamedTimerMetric(keyAndTimer.Key, keyAndTimer.Value.ElapsedMilliseconds));
	}

	public void ResetAllTimers()
	{
		foreach (TimerMetric value in _timerMetrics.Values)
		{
			value.Reset();
		}
	}
}
