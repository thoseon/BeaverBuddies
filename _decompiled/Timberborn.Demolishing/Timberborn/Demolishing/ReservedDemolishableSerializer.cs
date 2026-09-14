using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Demolishing;

public class ReservedDemolishableSerializer : IValueSerializer<ReservedDemolishable>
{
	private static readonly PropertyKey<Demolishable> DemolishableKey = new PropertyKey<Demolishable>("Demolishable");

	private static readonly PropertyKey<bool> ForceDemolishKey = new PropertyKey<bool>("ForceDemolish");

	private readonly ReferenceSerializer _referenceSerializer;

	public ReservedDemolishableSerializer(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Serialize(ReservedDemolishable value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(DemolishableKey, value.Demolishable, _referenceSerializer.Of<Demolishable>());
		objectSaver.Set(ForceDemolishKey, value.ForceDemolish);
	}

	public Obsoletable<ReservedDemolishable> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		if (objectLoader.GetObsoletable(DemolishableKey, _referenceSerializer.Of<Demolishable>(), out var value))
		{
			return new ReservedDemolishable(value, objectLoader.Get(ForceDemolishKey));
		}
		return null;
	}
}
