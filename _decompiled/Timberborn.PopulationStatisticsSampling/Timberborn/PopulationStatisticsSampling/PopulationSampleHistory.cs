using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.PopulationStatisticsSampling;

public class PopulationSampleHistory
{
	private readonly List<PopulationSample> _populationSamples;

	public ReadOnlyList<PopulationSample> PopulationSamples => _populationSamples.AsReadOnlyList();

	private PopulationSampleHistory(List<PopulationSample> populationSamples)
	{
		_populationSamples = populationSamples;
	}

	public static PopulationSampleHistory CreateNew()
	{
		return new PopulationSampleHistory(new List<PopulationSample>());
	}

	public static PopulationSampleHistory CreateFromSave(List<PopulationSample> populationSamples)
	{
		return new PopulationSampleHistory(populationSamples);
	}

	public void AddSample(PopulationSample populationSample)
	{
		_populationSamples.Add(populationSample);
	}
}
