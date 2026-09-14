using Timberborn.Persistence;

namespace Timberborn.Workshops;

public class DailyProductivitySerializer : IValueSerializer<DailyProductivity>
{
	private static readonly ListKey<HourlyProductivity> HourlyProductivitiesKey = new ListKey<HourlyProductivity>("HourlyProductivities");

	private static readonly PropertyKey<HourlyProductivity> CurrentProductivityKey = new PropertyKey<HourlyProductivity>("CurrentProductivity");

	private readonly HourlyProductivitySerializer _hourlyProductivitySerializer;

	public DailyProductivitySerializer(HourlyProductivitySerializer hourlyProductivitySerializer)
	{
		_hourlyProductivitySerializer = hourlyProductivitySerializer;
	}

	public void Serialize(DailyProductivity value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(HourlyProductivitiesKey, value.HourlyProductivities, _hourlyProductivitySerializer);
		objectSaver.Set(CurrentProductivityKey, value.CurrentProductivity, _hourlyProductivitySerializer);
	}

	public Obsoletable<DailyProductivity> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new DailyProductivity(objectLoader.Get(HourlyProductivitiesKey, _hourlyProductivitySerializer).ToArray(), objectLoader.Get(CurrentProductivityKey, _hourlyProductivitySerializer));
	}
}
