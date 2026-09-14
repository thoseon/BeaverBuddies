using System.Linq;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.PopulationStatisticsSampling;

public class GlobalPopulationSamplesRegistry : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey GlobalPopulationSamplesRegistryKey = new SingletonKey("GlobalPopulationSamplesRegistry");

	private static readonly PropertyKey<PackedList<PopulationSample>> SamplesKey = new PropertyKey<PackedList<PopulationSample>>("Samples");

	private readonly ISingletonLoader _singletonLoader;

	private readonly PopulationSamplePackedListSerializer _populationSamplePackedListSerializer;

	public PopulationSampleHistory PopulationSampleHistory { get; private set; }

	public GlobalPopulationSamplesRegistry(ISingletonLoader singletonLoader, PopulationSamplePackedListSerializer populationSamplePackedListSerializer)
	{
		_singletonLoader = singletonLoader;
		_populationSamplePackedListSerializer = populationSamplePackedListSerializer;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(GlobalPopulationSamplesRegistryKey).Set(SamplesKey, new PackedList<PopulationSample>(PopulationSampleHistory.PopulationSamples.ToArray()), _populationSamplePackedListSerializer);
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(GlobalPopulationSamplesRegistryKey, out var objectLoader))
		{
			PopulationSample[] array = objectLoader.Get(SamplesKey, _populationSamplePackedListSerializer).Array;
			PopulationSampleHistory = PopulationSampleHistory.CreateFromSave(array.ToList());
		}
		else
		{
			PopulationSampleHistory = PopulationSampleHistory.CreateNew();
		}
	}
}
