using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Timberborn.Benchmarking;

internal class BenchmarkParameters
{
	private readonly List<PerformanceSample> _performanceSamples;

	public int SamplingLengthInSeconds { get; }

	public int WarmUpLengthInSeconds { get; }

	public int GameSpeed { get; }

	public bool DetailedSamplesAvailable { get; }

	public ReadOnlyList<PerformanceSample> PerformanceSamples => _performanceSamples.AsReadOnlyList();

	public BenchmarkParameters(int samplingLengthInSeconds, int warmUpLengthInSeconds, int gameSpeed, IEnumerable<PerformanceSample> performanceSamples, bool detailedSamplesAvailable)
	{
		SamplingLengthInSeconds = samplingLengthInSeconds;
		WarmUpLengthInSeconds = warmUpLengthInSeconds;
		GameSpeed = gameSpeed;
		_performanceSamples = performanceSamples.ToList();
		DetailedSamplesAvailable = detailedSamplesAvailable;
	}
}
