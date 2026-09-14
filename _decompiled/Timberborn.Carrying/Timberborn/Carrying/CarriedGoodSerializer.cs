using Timberborn.Goods;
using Timberborn.Persistence;

namespace Timberborn.Carrying;

internal class CarriedGoodSerializer : IValueSerializer<CarriedGood>
{
	private static readonly PropertyKey<GoodAmount> GoodAmountKey = new PropertyKey<GoodAmount>("GoodAmount");

	private static readonly PropertyKey<CarriedGoodType> TypeKey = new PropertyKey<CarriedGoodType>("Type");

	private readonly GoodAmountSerializer _goodAmountSerializer;

	public CarriedGoodSerializer(GoodAmountSerializer goodAmountSerializer)
	{
		_goodAmountSerializer = goodAmountSerializer;
	}

	public void Serialize(CarriedGood value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(GoodAmountKey, value.GoodAmount, _goodAmountSerializer);
		objectSaver.Set(TypeKey, value.Type);
	}

	public Obsoletable<CarriedGood> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		if (objectLoader.GetObsoletable(GoodAmountKey, _goodAmountSerializer, out var value))
		{
			CarriedGoodType type = objectLoader.Get(TypeKey);
			return new CarriedGood(value, type);
		}
		return default(Obsoletable<CarriedGood>);
	}
}
