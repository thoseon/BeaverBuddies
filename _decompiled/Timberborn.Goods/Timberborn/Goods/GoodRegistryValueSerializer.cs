using Timberborn.Persistence;

namespace Timberborn.Goods;

public class GoodRegistryValueSerializer : IValueSerializer<GoodRegistry>
{
	private static readonly ListKey<GoodAmount> GoodsKey = new ListKey<GoodAmount>("Goods");

	private readonly GoodAmountSerializer _goodAmountSerializer;

	public GoodRegistryValueSerializer(GoodAmountSerializer goodAmountSerializer)
	{
		_goodAmountSerializer = goodAmountSerializer;
	}

	public void Serialize(GoodRegistry value, IValueSaver valueSaver)
	{
		valueSaver.AsObject().Set(GoodsKey, value.Goods, _goodAmountSerializer);
	}

	public Obsoletable<GoodRegistry> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		GoodRegistry goodRegistry = new GoodRegistry();
		foreach (GoodAmount item in objectLoader.Get(GoodsKey, _goodAmountSerializer))
		{
			goodRegistry.Add(item);
		}
		return goodRegistry;
	}
}
