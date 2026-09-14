using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.InventorySystem;

public class GoodReservationValueSerializer : IValueSerializer<GoodReservation>
{
	private static readonly PropertyKey<Inventory> InventoryKey = new PropertyKey<Inventory>("Inventory");

	private static readonly PropertyKey<GoodAmount> GoodAmountKey = new PropertyKey<GoodAmount>("GoodAmount");

	private static readonly PropertyKey<bool> FixedAmountKey = new PropertyKey<bool>("FixedAmount");

	private static readonly PropertyKey<bool> ConsumeGoodKey = new PropertyKey<bool>("ConsumeGood");

	private readonly GoodAmountSerializer _goodAmountSerializer;

	private readonly ReferenceSerializer _referenceSerializer;

	public GoodReservationValueSerializer(GoodAmountSerializer goodAmountSerializer, ReferenceSerializer referenceSerializer)
	{
		_goodAmountSerializer = goodAmountSerializer;
		_referenceSerializer = referenceSerializer;
	}

	public void Serialize(GoodReservation value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(InventoryKey, value.Inventory, _referenceSerializer.Of<Inventory>());
		objectSaver.Set(GoodAmountKey, value.GoodAmount, _goodAmountSerializer);
		objectSaver.Set(FixedAmountKey, value.FixedAmount);
		objectSaver.Set(ConsumeGoodKey, value.ConsumeGood);
	}

	[BackwardCompatible(2026, 5, 6, Compatibility.Save)]
	public Obsoletable<GoodReservation> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		if (objectLoader.GetObsoletable(GoodAmountKey, _goodAmountSerializer, out var value) && objectLoader.GetObsoletable(InventoryKey, _referenceSerializer.Of<Inventory>(), out var value2))
		{
			return new GoodReservation(value2, value, objectLoader.Get(FixedAmountKey), objectLoader.Has(ConsumeGoodKey) && objectLoader.Get(ConsumeGoodKey));
		}
		return default(Obsoletable<GoodReservation>);
	}
}
