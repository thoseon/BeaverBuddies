using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.PopulationStatisticsSampling;

public class DistrictPopulationSamplesRegistry : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, IPersistentEntity
{
	private static readonly ComponentKey DistrictPopulationSamplesRegistryKey = new ComponentKey("DistrictPopulationSamplesRegistry");

	private static readonly PropertyKey<PackedList<PopulationSample>> SamplesKey = new PropertyKey<PackedList<PopulationSample>>("Samples");

	private readonly PopulationSampler _populationSampler;

	private readonly PopulationSamplePackedListSerializer _populationSamplePackedListSerializer;

	public PopulationSampleHistory PopulationSampleHistory { get; private set; }

	public DistrictCenter DistrictCenter { get; private set; }

	public DistrictPopulationSamplesRegistry(PopulationSampler populationSampler, PopulationSamplePackedListSerializer populationSamplePackedListSerializer)
	{
		_populationSampler = populationSampler;
		_populationSamplePackedListSerializer = populationSamplePackedListSerializer;
	}

	public void Awake()
	{
		DistrictCenter = GetComponent<DistrictCenter>();
	}

	public void InitializeEntity()
	{
		if (PopulationSampleHistory == null)
		{
			PopulationSampleHistory populationSampleHistory = (PopulationSampleHistory = PopulationSampleHistory.CreateNew());
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(DistrictPopulationSamplesRegistryKey).Set(SamplesKey, new PackedList<PopulationSample>(PopulationSampleHistory.PopulationSamples.ToArray()), _populationSamplePackedListSerializer);
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DistrictPopulationSamplesRegistryKey, out var objectLoader))
		{
			PopulationSample[] array = objectLoader.Get(SamplesKey, _populationSamplePackedListSerializer).Array;
			PopulationSampleHistory = PopulationSampleHistory.CreateFromSave(array.ToList());
		}
	}

	public void OnEnterFinishedState()
	{
		_populationSampler.AddDistrictRegistry(this);
	}

	public void OnExitFinishedState()
	{
		_populationSampler.RemoveDistrictRegistry(this);
	}
}
