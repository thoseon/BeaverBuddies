using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timberborn.Metrics;

internal class MetricsFormatter
{
	public string FormatMetrics(IEnumerable<NamedMetricsContext> contexts)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (NamedMetricsContext context in contexts)
		{
			FormatContext(stringBuilder, context);
		}
		return stringBuilder.ToString();
	}

	private static void FormatContext(StringBuilder metrics, NamedMetricsContext context)
	{
		List<NamedTimerMetric> list = (from metric in context.GetAllTimers()
			orderby metric.ElapsedMilliseconds descending
			select metric).ToList();
		long num = list.Sum((NamedTimerMetric metric) => metric.ElapsedMilliseconds);
		metrics.AppendLine($"{context.Name} {(float)num / 1000f:0.00}s");
		metrics.AppendLine("Name,percentage of elapsed");
		foreach (NamedTimerMetric item in (IEnumerable<NamedTimerMetric>)list)
		{
			double num2 = (double)item.ElapsedMilliseconds / (double)num;
			metrics.AppendLine($"{item.Name},{num2:p}");
		}
		metrics.AppendLine();
	}
}
