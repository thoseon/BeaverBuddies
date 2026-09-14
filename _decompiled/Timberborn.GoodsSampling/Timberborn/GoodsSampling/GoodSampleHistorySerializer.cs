using System.Collections.Generic;
using Timberborn.Goods;
using Timberborn.Persistence;

namespace Timberborn.GoodsSampling;

public class GoodSampleHistorySerializer : IValueSerializer<GoodSampleHistory>
{
	private static readonly PropertyKey<SerializedGood> GoodKey = new PropertyKey<SerializedGood>("Good");

	private static readonly ListKey<int> CycleKey = new ListKey<int>("Cycle");

	private static readonly ListKey<int> DayKey = new ListKey<int>("Day");

	private static readonly ListKey<int> StockKey = new ListKey<int>("Stock");

	private static readonly ListKey<int> CapacityKey = new ListKey<int>("Capacity");

	private static readonly ListKey<int> ProductionKey = new ListKey<int>("Production");

	private static readonly ListKey<int> ConsumptionKey = new ListKey<int>("Consumption");

	private readonly SerializedGoodValueSerializer _serializedGoodValueSerializer;

	public GoodSampleHistorySerializer(SerializedGoodValueSerializer serializedGoodValueSerializer)
	{
		_serializedGoodValueSerializer = serializedGoodValueSerializer;
	}

	public void Serialize(GoodSampleHistory value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		List<int> list5 = new List<int>();
		List<int> list6 = new List<int>();
		foreach (GoodSample goodSample in value.GoodSamples)
		{
			list.Add(goodSample.Cycle);
			list2.Add(goodSample.Day);
			list3.Add(goodSample.Stock);
			list4.Add(goodSample.Capacity);
			list5.Add(goodSample.Production);
			list6.Add(goodSample.Consumption);
		}
		objectSaver.Set(GoodKey, new SerializedGood(value.GoodId), _serializedGoodValueSerializer);
		objectSaver.Set(CycleKey, list);
		objectSaver.Set(DayKey, list2);
		objectSaver.Set(StockKey, list3);
		objectSaver.Set(CapacityKey, list4);
		objectSaver.Set(ProductionKey, list5);
		objectSaver.Set(ConsumptionKey, list6);
	}

	public Obsoletable<GoodSampleHistory> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		if (!objectLoader.GetObsoletable(GoodKey, _serializedGoodValueSerializer, out var value))
		{
			return default(Obsoletable<GoodSampleHistory>);
		}
		return GoodSampleHistory.CreateFromSave(value.Id, UnpackGoodSamples(objectLoader));
	}

	private static List<GoodSample> UnpackGoodSamples(IObjectLoader objectLoader)
	{
		List<GoodSample> list = new List<GoodSample>();
		List<int> list2 = objectLoader.Get(CycleKey);
		List<int> list3 = objectLoader.Get(DayKey);
		List<int> list4 = objectLoader.Get(StockKey);
		List<int> list5 = objectLoader.Get(CapacityKey);
		List<int> list6 = objectLoader.Get(ProductionKey);
		List<int> list7 = objectLoader.Get(ConsumptionKey);
		for (int i = 0; i < list4.Count; i++)
		{
			list.Add(new GoodSample(list2[i], list3[i], list4[i], list5[i], list6[i], list7[i]));
		}
		return list;
	}
}
