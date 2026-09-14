using Timberborn.Persistence;

namespace Timberborn.Goods;

public class GoodAmountSerializer : IValueSerializer<GoodAmount>
{
	private static readonly PropertyKey<SerializedGood> GoodKey = new PropertyKey<SerializedGood>("Good");

	private static readonly PropertyKey<int> AmountKey = new PropertyKey<int>("Amount");

	private readonly SerializedGoodValueSerializer _serializedGoodValueSerializer;

	public GoodAmountSerializer(SerializedGoodValueSerializer serializedGoodValueSerializer)
	{
		_serializedGoodValueSerializer = serializedGoodValueSerializer;
	}

	public void Serialize(GoodAmount goodAmount, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(GoodKey, new SerializedGood(goodAmount.GoodId), _serializedGoodValueSerializer);
		objectSaver.Set(AmountKey, goodAmount.Amount);
	}

	public Obsoletable<GoodAmount> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		int amount = objectLoader.Get(AmountKey);
		if (!objectLoader.GetObsoletable(GoodKey, _serializedGoodValueSerializer, out var value))
		{
			return default(Obsoletable<GoodAmount>);
		}
		return new GoodAmount(value.Id, amount);
	}
}
