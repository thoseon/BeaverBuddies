using System.Collections.Generic;

namespace Timberborn.Metrics;

public class TimerMetricCache<T>
{
	private static readonly string ContextKey = typeof(T).Name;

	private readonly IMetricsService _metricsService;

	private readonly Dictionary<T, ITimerMetric> _timerMetrics = new Dictionary<T, ITimerMetric>();

	public TimerMetricCache(IMetricsService metricsService)
	{
		_metricsService = metricsService;
	}

	public ITimerMetric Get(T metricKey)
	{
		if (!_timerMetrics.TryGetValue(metricKey, out var value))
		{
			value = _metricsService.GetTimerMetric(ContextKey, metricKey.GetType().Name);
			_timerMetrics[metricKey] = value;
		}
		return value;
	}
}
