using Timberborn.Persistence;

namespace Timberborn.EntityNaming;

internal class SerializedEntityNameNumberSerializer : IValueSerializer<SerializedEntityNameNumber>
{
	private static readonly PropertyKey<string> GroupKey = new PropertyKey<string>("Group");

	private static readonly PropertyKey<int> NextNumberKey = new PropertyKey<int>("NextNumber");

	public void Serialize(SerializedEntityNameNumber value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(GroupKey, value.Group);
		objectSaver.Set(NextNumberKey, value.NextNumber);
	}

	public Obsoletable<SerializedEntityNameNumber> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new SerializedEntityNameNumber(objectLoader.Get(GroupKey), objectLoader.Get(NextNumberKey));
	}
}
