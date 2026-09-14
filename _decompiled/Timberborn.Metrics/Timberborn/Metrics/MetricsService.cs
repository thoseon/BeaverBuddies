using System.Collections.Generic;
using Timberborn.CommandLine;
using Timberborn.FileSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Metrics;

internal class MetricsService : IMetricsService, ILoadableSingleton
{
	private static readonly string MetricsKey = "metrics";

	private readonly MetricsRepository _metricsRepository;

	private readonly MetricsFormatter _metricsFormatter;

	private readonly IFileService _fileService;

	private readonly ICommandLineArguments _commandLineArguments;

	public bool MetricsEnabled { get; private set; }

	public MetricsService(MetricsRepository metricsRepository, MetricsFormatter metricsFormatter, IFileService fileService, ICommandLineArguments commandLineArguments)
	{
		_metricsRepository = metricsRepository;
		_metricsFormatter = metricsFormatter;
		_fileService = fileService;
		_commandLineArguments = commandLineArguments;
	}

	public void Load()
	{
		MetricsEnabled = _commandLineArguments.Has(MetricsKey);
	}

	public ITimerMetric GetTimerMetric(string contextKey, string timerKey)
	{
		return _metricsRepository.GetTimer(contextKey, timerKey);
	}

	public void ResetMetrics()
	{
		_metricsRepository.ResetAllTimers();
	}

	public void WriteCollectedDataToFile(string path)
	{
		if (MetricsEnabled)
		{
			IEnumerable<NamedMetricsContext> allContexts = _metricsRepository.GetAllContexts();
			string text = _metricsFormatter.FormatMetrics(allContexts);
			_fileService.WriteTextToFile(path + ".csv", text);
		}
	}
}
