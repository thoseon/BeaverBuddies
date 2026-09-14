using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.SoilMoistureSystem;

internal class WaterEvaporationMap : ISaveableSingleton, ILoadableSingleton, ITickableSingleton, IThreadSafeWaterEvaporationMap
{
	private static readonly SingletonKey WaterEvaporationMapKey = new SingletonKey("WaterEvaporationMap");

	private static readonly PropertyKey<PackedList<float>> EvaporationModifiersKey = new PropertyKey<PackedList<float>>("EvaporationModifiers");

	private static readonly PropertyKey<int> LevelsKey = new PropertyKey<int>("Levels");

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapIndexService _mapIndexService;

	private readonly FloatPackedListSerializer _floatPackedListSerializer;

	private readonly BufferedArray<float> _evaporationModifiers = new BufferedArray<float>();

	public float[] UnsafeEvaporationModifiers => _evaporationModifiers.Current;

	public ReadOnlyArray<float> EvaporationModifiers => new ReadOnlyArray<float>(_evaporationModifiers.Buffered);

	public WaterEvaporationMap(ISingletonLoader singletonLoader, MapIndexService mapIndexService, FloatPackedListSerializer floatPackedListSerializer)
	{
		_singletonLoader = singletonLoader;
		_mapIndexService = mapIndexService;
		_floatPackedListSerializer = floatPackedListSerializer;
	}

	public void Load()
	{
		_evaporationModifiers.Initialize(_mapIndexService.VerticalStride);
		_evaporationModifiers.Fill(1f);
		_evaporationModifiers.Unify();
	}

	[BackwardCompatible(2024, 1, 24, Compatibility.Map)]
	public void LoadData(ReadOnlyArray<byte> threadSafeColumnCounts)
	{
		if (!_singletonLoader.TryGetSingleton(WaterEvaporationMapKey, out var objectLoader))
		{
			return;
		}
		int levels = ((!objectLoader.Has(LevelsKey)) ? 1 : objectLoader.Get(LevelsKey));
		PackedList<float> packedList = objectLoader.Get(EvaporationModifiersKey, _floatPackedListSerializer);
		float[] array = _mapIndexService.Unpack(packedList, levels);
		int verticalStride = _mapIndexService.VerticalStride;
		int num = array.Length;
		foreach (int item in _mapIndexService.Indices2D)
		{
			for (int i = 0; i < threadSafeColumnCounts[item]; i++)
			{
				int num2 = i * verticalStride + item;
				if (num2 < num)
				{
					_evaporationModifiers.Current[num2] = array[num2];
				}
			}
		}
		_evaporationModifiers.Unify();
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(WaterEvaporationMapKey);
		int num = _evaporationModifiers.Current.Length / _mapIndexService.VerticalStride;
		singleton.Set(LevelsKey, num);
		singleton.Set(EvaporationModifiersKey, _mapIndexService.Pack(_evaporationModifiers.Current, num), _floatPackedListSerializer);
	}

	public void Tick()
	{
		_evaporationModifiers.Swap();
	}

	public void Resize(int maxColumnCount)
	{
		_evaporationModifiers.ResizeAndFill(_mapIndexService.VerticalStride * maxColumnCount, 1f);
	}
}
