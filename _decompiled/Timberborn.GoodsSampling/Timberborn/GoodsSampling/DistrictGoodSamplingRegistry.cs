using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.GoodsSampling;

public class DistrictGoodSamplingRegistry : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, IPersistentEntity
{
	private static readonly ComponentKey DistrictGoodSamplingRegistryKey = new ComponentKey("DistrictGoodSamplingRegistry");

	private static readonly PropertyKey<GoodSamplingRegistry> GoodSamplingRegistryKey = new PropertyKey<GoodSamplingRegistry>("GoodSamplingRegistry");

	private readonly GoodsSampler _goodsSampler;

	private readonly GoodSamplingRegistrySerializer _goodSamplingRegistrySerializer;

	private readonly IGoodService _goodService;

	public GoodSamplingRegistry GoodSamplingRegistry { get; private set; }

	public DistrictCenter DistrictCenter { get; private set; }

	internal DistrictGoodSamplingRegistry(GoodsSampler goodsSampler, GoodSamplingRegistrySerializer goodSamplingRegistrySerializer, IGoodService goodService)
	{
		_goodsSampler = goodsSampler;
		_goodSamplingRegistrySerializer = goodSamplingRegistrySerializer;
		_goodService = goodService;
	}

	public void Awake()
	{
		DistrictCenter = GetComponent<DistrictCenter>();
	}

	public void InitializeEntity()
	{
		if (GoodSamplingRegistry == null)
		{
			GoodSamplingRegistry goodSamplingRegistry = (GoodSamplingRegistry = GoodSamplingRegistry.CreateNew(_goodService.Goods));
		}
	}

	public void OnEnterFinishedState()
	{
		_goodsSampler.AddDistrictRegistry(this);
	}

	public void OnExitFinishedState()
	{
		_goodsSampler.RemoveDistrictRegistry(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(DistrictGoodSamplingRegistryKey).Set(GoodSamplingRegistryKey, GoodSamplingRegistry, _goodSamplingRegistrySerializer);
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DistrictGoodSamplingRegistryKey, out var objectLoader))
		{
			GoodSamplingRegistry = objectLoader.Get(GoodSamplingRegistryKey, _goodSamplingRegistrySerializer);
		}
	}
}
