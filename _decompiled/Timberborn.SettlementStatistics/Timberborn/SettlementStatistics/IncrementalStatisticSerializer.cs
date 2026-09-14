using Timberborn.Persistence;

namespace Timberborn.SettlementStatistics;

public class IncrementalStatisticSerializer : IValueSerializer<IncrementalStatistic>
{
	private static readonly PropertyKey<string> IdKey = new PropertyKey<string>("Id");

	private static readonly PropertyKey<int> ValueKey = new PropertyKey<int>("Value");

	public void Serialize(IncrementalStatistic value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(IdKey, value.Id);
		objectSaver.Set(ValueKey, value.Value);
	}

	public Obsoletable<IncrementalStatistic> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new IncrementalStatistic(objectLoader.Get(IdKey), objectLoader.Get(ValueKey));
	}
}
