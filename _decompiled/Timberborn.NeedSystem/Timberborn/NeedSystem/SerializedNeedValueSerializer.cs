using Timberborn.Persistence;

namespace Timberborn.NeedSystem;

public class SerializedNeedValueSerializer : IValueSerializer<SerializedNeed>
{
	private static readonly PropertyKey<string> NameKey = new PropertyKey<string>("Name");

	private static readonly PropertyKey<float> PointsKey = new PropertyKey<float>("Points");

	public void Serialize(SerializedNeed value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(NameKey, value.Id);
		objectSaver.Set(PointsKey, value.Points);
	}

	public Obsoletable<SerializedNeed> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new SerializedNeed(objectLoader.Get(NameKey), GetPoints(objectLoader));
	}

	private static float GetPoints(IObjectLoader objectLoader)
	{
		if (!objectLoader.Has(PointsKey))
		{
			return 1f;
		}
		return objectLoader.Get(PointsKey);
	}
}
