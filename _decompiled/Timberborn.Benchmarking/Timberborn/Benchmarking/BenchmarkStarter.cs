using Timberborn.CommandLine;
using Timberborn.SingletonSystem;

namespace Timberborn.Benchmarking;

internal class BenchmarkStarter : ILoadableSingleton
{
	private static readonly string BenchmarkLengthKey = "benchmarkLength";

	private static readonly string BenchmarkSpeedKey = "benchmarkSpeed";

	private static readonly string BenchmarkWarmUpLengthKey = "benchmarkWarmUpLength";

	private static readonly string BenchmarkSaveCountKey = "benchmarkSaveCount";

	private readonly ICommandLineArguments _commandLineArguments;

	private readonly Benchmarker _benchmarker;

	private readonly SavingBenchmarker _savingBenchmarker;

	public BenchmarkStarter(ICommandLineArguments commandLineArguments, Benchmarker benchmarker, SavingBenchmarker savingBenchmarker)
	{
		_commandLineArguments = commandLineArguments;
		_benchmarker = benchmarker;
		_savingBenchmarker = savingBenchmarker;
	}

	public void Load()
	{
		StartBenchmark();
	}

	private void StartBenchmark()
	{
		if (_commandLineArguments.Has(BenchmarkLengthKey) && _commandLineArguments.Has(BenchmarkSpeedKey))
		{
			int samplingLengthInSeconds = _commandLineArguments.GetInt(BenchmarkLengthKey);
			int gameSpeed = _commandLineArguments.GetInt(BenchmarkSpeedKey);
			int warmUpLengthInSeconds = _commandLineArguments.GetInt(BenchmarkWarmUpLengthKey);
			_benchmarker.StartBenchmark(samplingLengthInSeconds, warmUpLengthInSeconds, gameSpeed);
		}
		else if (_commandLineArguments.Has(BenchmarkSaveCountKey))
		{
			int warmUpLengthInSeconds2 = _commandLineArguments.GetInt(BenchmarkWarmUpLengthKey);
			int saveCount = _commandLineArguments.GetInt(BenchmarkSaveCountKey);
			_savingBenchmarker.StartBenchmark(warmUpLengthInSeconds2, saveCount);
		}
	}
}
