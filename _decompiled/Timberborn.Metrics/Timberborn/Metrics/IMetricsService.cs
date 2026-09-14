namespace Timberborn.Metrics;

public interface IMetricsService
{
	bool MetricsEnabled { get; }

	ITimerMetric GetTimerMetric(string contextKey, string timerKey);

	void ResetMetrics();

	void WriteCollectedDataToFile(string path);
}
