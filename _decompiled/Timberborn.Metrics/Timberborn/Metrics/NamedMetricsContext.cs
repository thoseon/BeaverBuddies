using System.Collections.Generic;

namespace Timberborn.Metrics;

internal class NamedMetricsContext
{
	private readonly MetricsContext _metricsContext;

	public string Name { get; }

	public NamedMetricsContext(string name, MetricsContext metricsContext)
	{
		Name = name;
		_metricsContext = metricsContext;
	}

	public IEnumerable<NamedTimerMetric> GetAllTimers()
	{
		return _metricsContext.GetAllTimers();
	}
}
