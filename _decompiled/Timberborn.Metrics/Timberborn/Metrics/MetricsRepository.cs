using System.Collections.Generic;
using System.Linq;

namespace Timberborn.Metrics;

internal class MetricsRepository
{
	private readonly Dictionary<string, MetricsContext> _contexts = new Dictionary<string, MetricsContext>();

	public ITimerMetric GetTimer(string contextKey, string timerKey)
	{
		if (!_contexts.TryGetValue(contextKey, out var value))
		{
			value = new MetricsContext();
			_contexts[contextKey] = value;
		}
		return value.GetTimer(timerKey);
	}

	public IEnumerable<NamedMetricsContext> GetAllContexts()
	{
		return _contexts.Select((KeyValuePair<string, MetricsContext> keyAndContext) => new NamedMetricsContext(keyAndContext.Key, keyAndContext.Value));
	}

	public void ResetAllTimers()
	{
		foreach (MetricsContext value in _contexts.Values)
		{
			value.ResetAllTimers();
		}
	}
}
