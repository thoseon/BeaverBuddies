using Timberborn.Persistence;

namespace Timberborn.HazardousWeatherSystem;

public class HazardousWeatherHistoryDataSerializer : IValueSerializer<HazardousWeatherHistoryData>
{
	private static readonly PropertyKey<string> HazardousWeatherIdKey = new PropertyKey<string>("HazardousWeatherId");

	private static readonly PropertyKey<int> DurationKey = new PropertyKey<int>("Duration");

	public void Serialize(HazardousWeatherHistoryData value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(HazardousWeatherIdKey, value.HazardousWeatherId);
		objectSaver.Set(DurationKey, value.Duration);
	}

	public Obsoletable<HazardousWeatherHistoryData> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new Obsoletable<HazardousWeatherHistoryData>(new HazardousWeatherHistoryData(objectLoader.Get(HazardousWeatherIdKey), objectLoader.Get(DurationKey)));
	}
}
