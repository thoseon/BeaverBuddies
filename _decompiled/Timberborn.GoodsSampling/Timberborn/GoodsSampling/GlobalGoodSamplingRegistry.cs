using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GoodsSampling;

public class GlobalGoodSamplingRegistry : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey GlobalGoodSamplingRegistryKey = new SingletonKey("GlobalGoodSamplingRegistry");

	private static readonly PropertyKey<GoodSamplingRegistry> GoodSamplingRegistryKey = new PropertyKey<GoodSamplingRegistry>("GoodSamplingRegistry");

	private readonly IGoodService _goodService;

	private readonly ISingletonLoader _singletonLoader;

	private readonly GoodSamplingRegistrySerializer _goodSamplingRegistrySerializer;

	public GoodSamplingRegistry GoodSamplingRegistry { get; private set; }

	public GlobalGoodSamplingRegistry(IGoodService goodService, ISingletonLoader singletonLoader, GoodSamplingRegistrySerializer goodSamplingRegistrySerializer)
	{
		_goodService = goodService;
		_singletonLoader = singletonLoader;
		_goodSamplingRegistrySerializer = goodSamplingRegistrySerializer;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(GlobalGoodSamplingRegistryKey).Set(GoodSamplingRegistryKey, GoodSamplingRegistry, _goodSamplingRegistrySerializer);
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(GlobalGoodSamplingRegistryKey, out var objectLoader))
		{
			GoodSamplingRegistry = objectLoader.Get(GoodSamplingRegistryKey, _goodSamplingRegistrySerializer);
		}
		else
		{
			GoodSamplingRegistry = GoodSamplingRegistry.CreateNew(_goodService.Goods);
		}
	}
}
