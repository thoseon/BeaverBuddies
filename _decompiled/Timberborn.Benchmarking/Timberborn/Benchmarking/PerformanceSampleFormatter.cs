using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Timberborn.Benchmarking;

internal class PerformanceSampleFormatter
{
	public string FormatAsCsv(IEnumerable<PerformanceSample> samples)
	{
		IEnumerable<string> values = Enumerable.Concat(second: samples.Select(SampleToRow), first: Enumerables.One("Time[s],Delta[s],Previous Update Tick length[s],Previous Update Base Length[s]"));
		return string.Join(Environment.NewLine ?? "", values);
	}

	private static string SampleToRow(PerformanceSample performanceSample)
	{
		float deltaInSeconds = performanceSample.DeltaInSeconds;
		double tickLengthInSeconds = performanceSample.TickLengthInSeconds;
		double num = (double)deltaInSeconds - tickLengthInSeconds;
		return $"{performanceSample.TimeInSeconds:0.0000}" + $",{deltaInSeconds:0.0000}" + $",{tickLengthInSeconds:0.0000}" + $",{num:0.0000}";
	}
}
